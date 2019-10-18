namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Json;
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
            var root = new BimDTO.BimGeneratorRoot();
            root.Model = new BimDTO.BimGeneratorModel();

            BimHelper.SetDefaultAnnotations(root.Model);
            BimHelper.SetDefaultDataSources(root.Model, DatabaseName);

            foreach (var tabledefeinition in databaseDefinition.GetTables())
            {
                root.Model.Tables.Add(GenerateTable(tabledefeinition));
            }

            ToJson(root);
        }

        private BimDTO.Table GenerateTable(SqlTable tabledefeinition)
        {
            var table = new BimDTO.Table();
            // TODO name wih schema
            table.Name = tabledefeinition.SchemaAndTableName.TableName;

            foreach (var columndefinition in tabledefeinition.Columns)
            {
                var column = new BimDTO.Column();
                // TODO mapping
                column.Name = columndefinition.Name;

                column.DataType = BimHelper.MapType(columndefinition.Type);
                column.SourceColumn = columndefinition.Name;

                table.Columns.Add(column);
            }

            BimHelper.SetDefaultPartition(table, tabledefeinition, DatabaseName);

            

            return table;
        }

        private void ToJson(BimDTO.BimGeneratorRoot root)
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.WriteIndented = true;
            jsonOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var json = JsonSerializer.Serialize(root, jsonOptions);

            WriteJson(json);
        }

        private void WriteJson(string json)
        {
            var folder = Path.Combine(DocumenterSettings.WorkingDirectory ?? @".\", DatabaseName);
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "Model.bim"), json, Encoding.UTF8);
        }
    }
}
