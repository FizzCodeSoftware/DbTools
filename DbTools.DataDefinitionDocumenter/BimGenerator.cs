namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter.BimDTO;
    using FizzCode.DbTools.Tabular;

    public class BimGenerator : DocumenterBase
    {
        public BimGenerator(DocumenterContext context, SqlVersion version, string databaseName)
            : base(context, version, databaseName)
        {
        }

        public void Generate(DatabaseDefinition databaseDefinition)
        {
            Log(LogSeverity.Information, "Starting on {DatabaseName}.", "BimGenerator", DatabaseName);

            var root = new BimGeneratorRoot
            {
                Model = new BimGeneratorModel()
            };

            BimHelper.SetDefaultAnnotations(root.Model);
            BimHelper.SetDefaultDataSources(root.Model, DatabaseName);

            var relationShipRegistrations = new RelationShipRegistrations();

            foreach (var sqlTable in databaseDefinition.GetTables())
            {
                if (!Context.Customizer.ShouldSkip(sqlTable.SchemaAndTableName)
                    && !Documenter.ShouldSkipKnownTechnicalTable(sqlTable.SchemaAndTableName)
                    )
                {
                    root.Model.Tables.Add(GenerateTable(sqlTable));
                    Context.Logger.Log(LogSeverity.Debug, "Table {TableName} added to bim model.", "BimGenerator", sqlTable.SchemaAndTableName);
                    GatherReferencedTablesByFK(relationShipRegistrations, sqlTable);
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
                var toTables = new Dictionary<string, int>();
                var to = relationShipRegistrations.GetByFromTable(fromTable);
                foreach (var rr in to)
                {
                    if (rr.FromTableSchemaAndTableName == rr.ToTableSchemaAndTableName)
                    {
                        Context.Logger.Log(LogSeverity.Warning, "Table {TableName} is referencing itself. SKIPPED.", "BimGenerator", fromTable);
                        continue;
                    }

                    var trp = rr.FromColumn.Properties.OfType<TabularRelationProperty>().FirstOrDefault();

                    if (trp != null)
                        rr.RelationshipIdentifier = trp.RelationshipIdentifier;

                    // TODO TabularRelationProperty without FK should create replationship
                    // TODO IF we have a RelationshipIdentifier
                    //  A/ Create a Copy -> TableName + " " + RelationshipIdentifier
                    //   if any other reference exists (without RelationshipIdentifier)
                    //  B/ Rename existing TableName to TableName + " " + RelationshipIdentifier
                    //   if NO other reference exists (without RelationshipIdentifier or any other RelationshipIdentifier)

                    // same target on this table
                    if (!toTables.ContainsKey(rr.ToKey))
                    {
                        toTables.Add(rr.ToKey, 1);
                    }
                    else
                    {
                        toTables[rr.ToKey]++;
                        var numberOfReferencesToTheSameTable = toTables[rr.ToKey];
                        if (numberOfReferencesToTheSameTable > 1)
                            CreateTableCopyForReference(rr, model, numberOfReferencesToTheSameTable);
                    }

                    model.Relationships.Add(GenerateRelationship(rr));
                }
            }
        }

        private void CreateTableCopyForReference(BimRelationship rr, BimGeneratorModel model, int i)
        {
            var dd = rr.FromColumn.Table.DatabaseDefinition;
            var toSqlTable = dd.GetTable(rr.ToTableSchemaAndTableName);

            var suffix = " " + i;
            if (rr.RelationshipIdentifier != null)
                suffix = " " + rr.RelationshipIdentifier;

            var copyTableName = GetBimTableName(toSqlTable.SchemaAndTableName) + suffix;

            var copySchemaAndTableName = new SchemaAndTableName(toSqlTable.SchemaAndTableName.Schema, toSqlTable.SchemaAndTableName.TableName + suffix);
            rr.ToTableSchemaAndTableName = copySchemaAndTableName;

            if (!model.Tables.Any(t => t.Name == copyTableName))
            {
                Context.Logger.Log(LogSeverity.Information, "Table {TableName} COPIED to bim model, multiple references from {FromTable}", "BimGenerator", copySchemaAndTableName, rr.FromColumn.Table.SchemaAndTableName);
                model.Tables.Add(GenerateTable(toSqlTable, copyTableName));
            }
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

        private void GatherReferencedTablesByFK(RelationShipRegistrations relationShipRegistrations, SqlTable sqlTable)
        {
            // TODO order of relationships from FKs should follow column (declaration) order.
            var fks = sqlTable.Properties.OfType<ForeignKey>();
            foreach (var fk in fks)
            {
                var firstColumnMap = fk.ForeignKeyColumns[0];

                var bimRelationship = new BimRelationship(firstColumnMap.ForeignKeyColumn, firstColumnMap.ReferredColumn.Table.SchemaAndTableName, firstColumnMap.ReferredColumn.Name);

                if (relationShipRegistrations.Contains(bimRelationship))
                    continue;

                relationShipRegistrations.Add(new BimRelationship(firstColumnMap.ForeignKeyColumn, firstColumnMap.ReferredColumn.Table.SchemaAndTableName, firstColumnMap.ReferredColumn.Name));
                GatherReferencedTablesByFK(relationShipRegistrations, fk.ReferredTable);
            }
        }

        private static string GetBimTableName(SchemaAndTableName schemaAndTableName)
        {
            // TODO A/ leave out default schema B/ option to leave out if all schema are the same C/ namingstrategy
            if (string.IsNullOrEmpty(schemaAndTableName.Schema))
                return schemaAndTableName.TableName;

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
                    DataType = sqlColumn.Types[Version].SqlTypeInfo.SqlDataType,
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
            var folder = Path.Combine(Context.DocumenterSettings.WorkingDirectory ?? @".\", DatabaseName);

            Context.Logger.Log(LogSeverity.Information, "Writing Json file {FileName} to folder {Folder}", "BimGenerator", "Model.bim", folder);

            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "Model.bim"), json, Encoding.UTF8);
        }
    }
}
