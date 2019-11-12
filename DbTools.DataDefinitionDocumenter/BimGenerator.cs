namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter.BimDTO;
    using FizzCode.DbTools.Tabular;

    public class BimGenerator : DocumenterBase
    {
        public BimGenerator(DocumenterSettings documenterSettings, Settings settings, string databaseName, ITableCustomizer tableCustomizer = null)
            : base(documenterSettings, settings, databaseName, tableCustomizer)
        {
        }

        public void Generate(DatabaseDefinition databaseDefinition)
        {
            var root = new BimGeneratorRoot
            {
                Model = new BimGeneratorModel()
            };

            BimHelper.SetDefaultAnnotations(root.Model);
            BimHelper.SetDefaultDataSources(root.Model, DatabaseName);

            var relationShipRegistrations = new RelationShipRegistrations();

            foreach (var sqlTable in databaseDefinition.GetTables())
            {
                if (!TableCustomizer.ShouldSkip(sqlTable.SchemaAndTableName)
                    && !Documenter.ShouldSkipKnownTechnicalTable(sqlTable.SchemaAndTableName)
                    )
                {
                    root.Model.Tables.Add(GenerateTable(sqlTable));
                    GatherReferencedTables(relationShipRegistrations, sqlTable);
                }
            }

            GenerateReferences(relationShipRegistrations, root.Model);

            var jsonString = ToJson(root);
            jsonString = RemoveInvalidEmptyItems(jsonString);
            WriteJson(jsonString);
        }

        private void GenerateReferences(RelationShipRegistrations relationShipRegistrations, BimGeneratorModel model)
        {
            // Create shadow copies where multiple FKs are on a source table
            // Use same RelationShipIdentifier
            //  Omit Source table if no other FK is pointing to it

            foreach (var fromTable in relationShipRegistrations.FromTables())
            {
                var to = relationShipRegistrations.GetByFromTable(fromTable);
                var i = 1;
                var count = to.Values.Count;
                foreach (var rr in to.Values)
                {
                    // TODO same target on this table
                    // TODO do not create already existing "table N" if exists

                    var trp = rr.FromColumn.Properties.OfType<TabularRelationProperty>().FirstOrDefault();

                    if (trp != null)
                        rr.RelationshipIdentifier = trp.RelationshipIdentifier;

                    if (count == 1)
                    {
                        model.Relationships.Add(GenerateRelationship(rr));
                    }
                    else
                    {
                        if (i > 1)
                        {
                            CreateTableCopyForReference(rr, model, i);
                            model.Relationships.Add(GenerateRelationship(rr));
                        }
                        else
                            model.Relationships.Add(GenerateRelationship(rr));
                    }

                    i++;
                }
            }
        }

        private void CreateTableCopyForReference(BimRelationship rr, BimGeneratorModel model, int i)
        {
            var dd = rr.FromColumn.Table.DatabaseDefinition;
            var toSqlTable = dd.GetTable(rr.ToTableSchemaAndTableName);

            var suffix = " " + i;
            if(rr.RelationshipIdentifier != null)
                suffix = " " + rr.RelationshipIdentifier;

            var copyTableName = GetBimTableName(toSqlTable.SchemaAndTableName) + suffix;

            var copySchemaAndTableName = new SchemaAndTableName(toSqlTable.SchemaAndTableName.Schema, toSqlTable.SchemaAndTableName.TableName + suffix);
            rr.ToTableSchemaAndTableName = copySchemaAndTableName;

            if (!model.Tables.Any(t => t.Name == copyTableName))
                model.Tables.Add(GenerateTable(toSqlTable, copyTableName));
        }

        private static Relationship GenerateRelationship(BimRelationship rr)
        {
            var relation = new Relationship
            {
                FromTable = GetBimTableName(rr.FromTableSchemaAndTableName),
                FromColumn = rr.FromColumn.Name,
                ToTable = GetBimTableName(rr.ToTableSchemaAndTableName),
                ToColumn = rr.ToColumnName,
                Name = Guid.NewGuid().ToString()
            };

            return relation;
        }

        private void GatherReferencedTables(RelationShipRegistrations relationShipRegistrations, SqlTable sqlTable)
        {
            // TODO circular dependencies

            // TODO order of relationships from FKs should follow column (devlaration) order.
            var fks = sqlTable.Properties.OfType<ForeignKey>();
            foreach (var fk in fks)
            {
                var firstColumnMap = fk.ForeignKeyColumns.First();

                var bimRelationship = new BimRelationship(firstColumnMap.ForeignKeyColumn, firstColumnMap.ReferredColumn.Table.SchemaAndTableName, firstColumnMap.ReferredColumn.Name);

                if (relationShipRegistrations.Contains(bimRelationship))
                    continue;

                relationShipRegistrations.Add(new BimRelationship(firstColumnMap.ForeignKeyColumn, firstColumnMap.ReferredColumn.Table.SchemaAndTableName, firstColumnMap.ReferredColumn.Name));
                GatherReferencedTables(relationShipRegistrations, fk.ReferredTable);
            }
        }

        private static string GetBimTableName(SchemaAndTableName schemaAndTableName)
        {
            // TODO A/ leave out default schema B/ option to leave out if all schema are the same C/ namingstrategy
            if (string.IsNullOrEmpty(schemaAndTableName.Schema))
                return schemaAndTableName.TableName;
            else
                return schemaAndTableName.Schema + " " + schemaAndTableName.TableName;
        }

        private Table GenerateTable(SqlTable sqlTable, string overrideName = null)
        {
            var table = new Table
            {
                Name = GetBimTableName(sqlTable.SchemaAndTableName)
            };

            if (overrideName != null)
                table.Name = overrideName;

            foreach (var sqlColumn in sqlTable.Columns)
            {
                var column = new Column
                {
                    // TODO mapping
                    Name = sqlColumn.Name,
                    DataType = BimHelper.MapType(sqlColumn.Type),
                    SourceColumn = sqlColumn.Name
                };

                table.Columns.Add(column);
            }

            BimHelper.SetDefaultPartition(table, sqlTable, DatabaseName);

            return table;
        }

        private static string ToJson(BimGeneratorRoot root)
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
