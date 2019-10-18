namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.IO;
    using System.Text;
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
            // Test one table now
            var tabledefeinition = databaseDefinition.GetTables()[0];

            var table = new BimDTO.Table();
            // TODO name wih schema
            table.Name = tabledefeinition.SchemaAndTableName.TableName;

            foreach (var columndefinition in tabledefeinition.Columns)
            {
                var column = new BimDTO.Column();
                // TODO mapping
                column.Name = columndefinition.Name;
                column.DataType = columndefinition.Type.ToString();
                column.SourceColumn = columndefinition.Name;

                table.Columns.Add(column);
            }

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.WriteIndented = true;
            jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var json = JsonSerializer.Serialize(table, jsonOptions);

            var folder = Path.Combine(DocumenterSettings.WorkingDirectory ?? @".\", DatabaseName);
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "Model.bim"), json, Encoding.UTF8);
        }
    }
}
