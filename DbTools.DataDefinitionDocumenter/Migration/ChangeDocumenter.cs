namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;

    public class ChangeDocumenter : DocumenterWriterBase
    {
        protected string OriginalDatabaseName => DatabaseName;

        protected string NewDatabaseName { get; }

        public new ChangeDocumenterContext Context { get;  }

        public ChangeDocumenter(ChangeDocumenterContext context, SqlEngineVersion version, string originalDatabaseName = "", string newDatabaseName = "", string fileName = null)
            : this(new DocumenterWriterExcel(), context, version, originalDatabaseName, newDatabaseName, fileName)
        {
        }

        public ChangeDocumenter(IDocumenterWriter documenterWriter, ChangeDocumenterContext context, SqlEngineVersion version, string originalDatabaseName = "", string newDatabaseName = "", string fileName = null)
            : base(documenterWriter, context, version, originalDatabaseName, fileName)
        {
            Context = context;
            NewDatabaseName = newDatabaseName;
        }

        private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategoryOrignal = new List<KeyValuePair<string, SqlTable>>();
        private readonly List<KeyValuePair<string, SqlTable>> _skippedSqlTablesByCategoryOriginal = new List<KeyValuePair<string, SqlTable>>();

        private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategoryNew = new List<KeyValuePair<string, SqlTable>>();
        private readonly List<KeyValuePair<string, SqlTable>> _skippedSqlTablesByCategoryNew = new List<KeyValuePair<string, SqlTable>>();

        public void Document(DatabaseDefinition originalDd, DatabaseDefinition newDd)
        {
            Log(LogSeverity.Information, "Starting on {OriginalDatabaseName} vs. {NewDatabaseName}", "Documenter", OriginalDatabaseName, NewDatabaseName);

            var tablesOriginal = RemoveKnownTechnicalTables(originalDd.GetTables());

            foreach (var table in tablesOriginal)
            {
                if (!Context.CustomizerOriginal.ShouldSkip(table.SchemaAndTableName))
                    _sqlTablesByCategoryOrignal.Add(new KeyValuePair<string, SqlTable>(Context.CustomizerOriginal.Category(table.SchemaAndTableName), table));
                else
                    _skippedSqlTablesByCategoryOriginal.Add(new KeyValuePair<string, SqlTable>(Context.CustomizerOriginal.Category(table.SchemaAndTableName), table));
            }

            var tablesNew = RemoveKnownTechnicalTables(newDd.GetTables());

            foreach (var table in tablesNew)
            {
                if (!Context.CustomizerNew.ShouldSkip(table.SchemaAndTableName))
                    _sqlTablesByCategoryOrignal.Add(new KeyValuePair<string, SqlTable>(Context.CustomizerNew.Category(table.SchemaAndTableName), table));
                else
                    _skippedSqlTablesByCategoryOriginal.Add(new KeyValuePair<string, SqlTable>(Context.CustomizerNew.Category(table.SchemaAndTableName), table));
            }

            WriteLine("Database", "", "Original", "New");
            WriteLine("Database", "Database name", OriginalDatabaseName, NewDatabaseName);

            var noOfTablesOriginal = originalDd.GetTables().Count;
            var noOfNotSkippedTablesOriginal = originalDd.GetTables().Count(t => !Context.CustomizerOriginal.ShouldSkip(t.SchemaAndTableName));
            var noOfTablesNew = newDd.GetTables().Count;
            var noOfNotSkippedTablesNew = newDd.GetTables().Count(t => !Context.CustomizerNew.ShouldSkip(t.SchemaAndTableName));

            WriteLine("Database", "Number of documented tables", noOfNotSkippedTablesOriginal, noOfNotSkippedTablesNew);
            WriteLine("Database", "Number of skipped tables", noOfTablesOriginal - noOfNotSkippedTablesOriginal, noOfTablesNew - noOfNotSkippedTablesNew);
            WriteLine("Database", "Number of tables", noOfTablesOriginal, noOfTablesNew);

            var comparer = new Comparer(Context);
            var changes = comparer.Compare(originalDd, newDd);

            // deleted tables
            // new tables
            // changes
            WriteLine("Tables", "Schema", "Table Name", "Event");

            foreach (var tableDelete in changes.OfType<TableDelete>())
            {
                if(!Context.CustomizerOriginal.ShouldSkip(tableDelete.SchemaAndTableName))
                    WriteLine("Tables", tableDelete.SchemaAndTableName.Schema, tableDelete.SchemaAndTableName.TableName, "Deleted");
            }

            foreach (var tableNew in changes.OfType<TableNew>())
            {
                if (!Context.CustomizerNew.ShouldSkip(tableNew.SchemaAndTableName))
                    WriteLine("Tables", tableNew.SchemaAndTableName.Schema, tableNew.SchemaAndTableName.TableName, "Added");
            }

            var processedTables = new List<SchemaAndTableName>();

            foreach (var change in changes.Where(c => !(c is TableNew )&& !(c is TableDelete)))
            {
                switch (change)
                {
                    //"Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column"

                    case ColumnNew column:
                        {
                            if (Context.CustomizerNew.ShouldSkip(column.Table.SchemaAndTableName))
                                continue;

                            ProcessTable(processedTables, column.Table);
                            // TODO internal data types are not OK this way
                            var pks = column.Table.Properties.OfType<PrimaryKey>().ToList();
                            var columnDocumentInfo = GetColumnDocumentInfo(pks, column);
                            WriteLine(column.Table.SchemaAndTableName, "New"); // TODO
                            AddColumnsToTableShet(column, columnDocumentInfo);
                            break;
                        }
                    case ColumnDelete column:
                        {
                            if (Context.CustomizerNew.ShouldSkip(column.Table.SchemaAndTableName))
                                continue;

                            ProcessTable(processedTables, column.Table);
                            // TODO internal data types are not OK this way
                            var pks = column.Table.Properties.OfType<PrimaryKey>().ToList();
                            var columnDocumentInfo = GetColumnDocumentInfo(pks, column);
                            WriteLine(column.Table.SchemaAndTableName, "New"); // TODO
                            AddColumnsToTableShet(column, columnDocumentInfo);
                            break;
                        }
                    /*default:
                        throw new NotImplementedException();*/
                }
            }

            Log(LogSeverity.Information, "Generating Document content.", "ChangeDocumenter");
            var content = DocumenterWriter.GetContent();

            var fileName = FileName ?? (OriginalDatabaseName == null && NewDatabaseName == null
                    ? "DatabaseChanges.xlsx"
                    : $"{OriginalDatabaseName}_vs_{NewDatabaseName}.xlsx");

            var path = Context.DocumenterSettings?.WorkingDirectory;

            Log(LogSeverity.Information, "Writing Document file {FileName} to folder {Folder}", "Documenter", fileName, path);

            if (!string.IsNullOrEmpty(path))
            {
                fileName = Path.Combine(path, fileName);
            }

            File.WriteAllBytes(fileName, content);
        }

        private void ProcessTable(List<SchemaAndTableName> processedTables, SqlTable table)
        {
            if(!processedTables.Contains(table.SchemaAndTableName))
            {
                processedTables.Add(table.SchemaAndTableName);
                // TODO category
                AddTableHeader(false, null, table);
            }
        }

        protected override Color? GetColor(SchemaAndTableName schemaAndTableName)
        {
            // TODO coloring to incude schema
            var hexColor = Context.CustomizerNew.BackGroundColor(schemaAndTableName);

            if (hexColor == null)
                return null;

            return ColorTranslator.FromHtml(hexColor);
        }
    }
}
