namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;

    public class BimGenerator : DocumenterBase
    {
        public BimGenerator(DocumenterSettings documenterSettings, Settings settings, string databaseName, ITableCustomizer tableCustomizer = null)
            : base(documenterSettings, settings, databaseName, tableCustomizer)
        {
        }

        public void Generate(DatabaseDefinition databaseDefinition)
        {
            var root = new BimDTO.BimGeneratorRoot
            {
                Model = new BimDTO.BimGeneratorModel()
            };

            BimHelper.SetDefaultAnnotations(root.Model);
            BimHelper.SetDefaultDataSources(root.Model, DatabaseName);

            foreach (var tableDefinition in databaseDefinition.GetTables())
            {
                if (!TableCustomizer.ShouldSkip(tableDefinition.SchemaAndTableName))
                    root.Model.Tables.Add(GenerateTable(tableDefinition));
            }

            var jsonString = ToJson(root);
            jsonString = RemoveInvalidEmptyItems(jsonString);
            WriteJson(jsonString);
        }

        private BimDTO.Table GenerateTable(SqlTable tabledefeinition)
        {
            var table = new BimDTO.Table
            {
                // TODO name wih schema
                Name = tabledefeinition.SchemaAndTableName.TableName
            };

            foreach (var columndefinition in tabledefeinition.Columns)
            {
                var column = new BimDTO.Column
                {
                    // TODO mapping
                    Name = columndefinition.Name,

                    DataType = BimHelper.MapType(columndefinition.Type),
                    SourceColumn = columndefinition.Name
                };

                table.Columns.Add(column);
            }

            BimHelper.SetDefaultPartition(table, tabledefeinition, DatabaseName);

            return table;
        }

        private static string ToJson(BimDTO.BimGeneratorRoot root)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(root, jsonOptions);

            return json;
        }

        private static string RemoveInvalidEmptyItems(string jsonString)
        {
            jsonString = Regex.Replace(jsonString, ".*\"formatString\": null[,]?\r\n", "");
            jsonString = Regex.Replace(jsonString, ".*\"annotations\": \\[\\][,]?\r\n", "");

            return jsonString;
        }

        private void WriteJson(string json)
        {
            var folder = Path.Combine(DocumenterSettings.WorkingDirectory ?? @".\", DatabaseName);
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "Model.bim"), json, Encoding.UTF8);
        }
    }
}
