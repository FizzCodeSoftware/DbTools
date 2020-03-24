namespace FizzCode.DbTools.DataDefinitionDocumenter
{
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

        public new ChangeDocumenterContext Context { get; }

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

        //private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategoryNew = new List<KeyValuePair<string, SqlTable>>();
        //private readonly List<KeyValuePair<string, SqlTable>> _skippedSqlTablesByCategoryNew = new List<KeyValuePair<string, SqlTable>>();

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
                if (!Context.CustomizerOriginal.ShouldSkip(tableDelete.SchemaAndTableName))
                    WriteLine("Tables", tableDelete.SchemaAndTableName.Schema, tableDelete.SchemaAndTableName.TableName, "Deleted");
            }

            foreach (var tableNew in changes.OfType<TableNew>())
            {
                if (!Context.CustomizerNew.ShouldSkip(tableNew.SchemaAndTableName))
                    WriteLine("Tables", tableNew.SchemaAndTableName.Schema, tableNew.SchemaAndTableName.TableName, "Added");
            }

            var processedTables = new List<SchemaAndTableName>();

            foreach (var change in changes.OfType<ColumnMigration>())
            {
                switch (change)
                {
                    //"Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column"

                    case ColumnNew column:
                        {
                            if (Context.CustomizerNew.ShouldSkip(column.Table.SchemaAndTableName))
                                continue;

                            ProcessColumnMigration(processedTables, column, "New");
                            break;
                        }
                    case ColumnDelete column:
                        {
                            if (Context.CustomizerOriginal.ShouldSkip(column.Table.SchemaAndTableName))
                                continue;

                            ProcessColumnMigration(processedTables, column, "Delete");
                            break;
                        }
                    case ColumnChange column:
                        {
                            if (Context.CustomizerNew.ShouldSkip(column.NewNameAndType.Table.SchemaAndTableName))
                                continue;

                            ProcessColumnMigration(processedTables, column, "Original");
                            ProcessColumnMigration(processedTables, column.NewNameAndType, "Changed to");
                            break;
                        }
                }
            }

            if (!Context.DocumenterSettings.NoForeignKeys)
            {
                var processedFKs = new List<SchemaAndTableName>();

                foreach (var change in changes.OfType<ForeignKeyMigration>())
                {
                    ProcessTable(processedTables, change.ForeignKey.SqlTable); // Ensure table header

                    switch (change)
                    {
                        case ForeignKeyNew fkNew:
                            {
                                if (Context.CustomizerNew.ShouldSkip(fkNew.ForeignKey.ReferredTable.SchemaAndTableName))
                                    continue;

                                ProcessForeignKey(processedFKs, fkNew.ForeignKey, "New");
                                break;
                            }
                        case ForeignKeyDelete fkDelete:
                            {
                                if (Context.CustomizerOriginal.ShouldSkip(fkDelete.ForeignKey.ReferredTable.SchemaAndTableName))
                                    continue;

                                ProcessForeignKey(processedFKs, fkDelete.ForeignKey, "Delete");

                                break;
                            }
                        case ForeignKeyChange fkChange:
                            {
                                if (Context.CustomizerNew.ShouldSkip(fkChange.NewForeignKey.ReferredTable.SchemaAndTableName))
                                    continue;

                                ProcessForeignKey(processedFKs, fkChange.ForeignKey, "Original");
                                ProcessForeignKey(processedFKs, fkChange.NewForeignKey, "Change to");

                                break;
                            }
                    }
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

        private void ProcessForeignKey(List<SchemaAndTableName> processedFKs, ForeignKey fk, string firstColumn)
        {
            if (!processedFKs.Contains(fk.SqlTable.SchemaAndTableName))
            {
                processedFKs.Add(fk.SqlTable.SchemaAndTableName);

                var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

                WriteLine(fk.SqlTable.SchemaAndTableName);

                WriteAndMerge(fk.SqlTable.SchemaAndTableName, mergeAmount, "Foreign keys");
                WriteLine(fk.SqlTable.SchemaAndTableName);

                // TODO allow nulls. Check / other properties?
                WriteLine(fk.SqlTable.SchemaAndTableName, "Event", "Foreign key name", "Column", "Referenced Table", "link", "Referenced Column");
            }

            AddForeignKey(fk, firstColumn);
        }

        private void ProcessColumnMigration(List<SchemaAndTableName> processedTables, SqlColumn column, string firstColumn)
        {
            ProcessTable(processedTables, column.Table);
            var pks = column.Table.Properties.OfType<PrimaryKey>().ToList();
            var columnDocumentInfo = GetColumnDocumentInfo(pks, column);
            AddColumnsToTableSheet(column, columnDocumentInfo, firstColumn);
        }

        private void ProcessTable(List<SchemaAndTableName> processedTables, SqlTable table)
        {
            if (!processedTables.Contains(table.SchemaAndTableName))
            {
                processedTables.Add(table.SchemaAndTableName);
                // TODO category
                AddTableHeader(false, null, table, "Event");
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
