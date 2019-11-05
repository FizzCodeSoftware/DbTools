namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter.BimDTO;

    public class BimGenerator : DocumenterBase
    {
        public BimGenerator(DocumenterSettings documenterSettings, Settings settings, string databaseName, ITableCustomizer tableCustomizer = null)
            : base(documenterSettings, settings, databaseName, tableCustomizer)
        {
        }

        public void Generate(DatabaseDefinition databaseDefinition)
        {
            var sqlTables = new List<SqlTable>();

            var root = new BimGeneratorRoot
            {
                Model = new BimGeneratorModel()
            };

            BimHelper.SetDefaultAnnotations(root.Model);
            BimHelper.SetDefaultDataSources(root.Model, DatabaseName);

            foreach (var sqlTable in databaseDefinition.GetTables())
            {
                if (!TableCustomizer.ShouldSkip(sqlTable.SchemaAndTableName))
                {
                    if (!root.Model.Tables.Any(t => t.Name == sqlTable.SchemaAndTableName.TableName))
                    {
                        root.Model.Tables.Add(GenerateTable(sqlTable));
                        AddReferencedTables(sqlTable, root.Model);
                    }
                }
            }

            var jsonString = ToJson(root);
            jsonString = RemoveInvalidEmptyItems(jsonString);
            WriteJson(jsonString);
        }

        private void AddReferencedTables(SqlTable sqlTable, BimGeneratorModel model)
        {
            // TODO circular dependencies

            var fks = sqlTable.Properties.OfType<ForeignKey>();
            foreach (var fk in fks)
            {
                if (!model.Tables.Any(t => t.Name == fk.ReferredTable.SchemaAndTableName.TableName))
                {
                    model.Tables.Add(GenerateTable(fk.ReferredTable));
                    model.Relationships.Add(GenerateReference(fk, sqlTable));
                    AddReferencedTables(fk.ReferredTable, model);
                }
            }
        }

        private Relationship GenerateReference(ForeignKey fk, SqlTable sqlTable)
        {
            // TODO handle TabularRelationProperty
            // TODO handle N:1 referrences - COPY tables

            var relation = new Relationship
            {
                FromTable = fk.SqlTable.SchemaAndTableName.TableName,
                FromColumn = fk.ForeignKeyColumns.First().ForeignKeyColumn.Name,
                ToTable = fk.ReferredTable.SchemaAndTableName.TableName,
                ToColumn = fk.ForeignKeyColumns.First().ReferredColumn.Name
            };
            
            return relation;
        }

        private BimDTO.Table GenerateTable(SqlTable tabledefeinition)
        {
            var table = new Table
            {
                // TODO name wih schema
                Name = tabledefeinition.SchemaAndTableName.TableName
            };

            foreach (var columndefinition in tabledefeinition.Columns)
            {
                var column = new Column
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
