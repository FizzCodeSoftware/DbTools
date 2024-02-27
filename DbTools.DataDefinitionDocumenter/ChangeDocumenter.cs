using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Migration;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class ChangeDocumenter(IDocumenterWriter documenterWriter, ChangeDocumenterContext context, SqlEngineVersion version, string originalDatabaseName = "", string newDatabaseName = "", string? fileName = null)
    : DocumenterWriterBase(documenterWriter, context, version, originalDatabaseName, fileName)
{
    protected string OriginalDatabaseName => DatabaseName;

    protected string NewDatabaseName { get; } = newDatabaseName;

    public new ChangeDocumenterContext Context { get; } = context;

    public ChangeDocumenter(ChangeDocumenterContext context, SqlEngineVersion version, string originalDatabaseName = "", string newDatabaseName = "", string? fileName = null)
        : this(new DocumenterWriterExcel(), context, version, originalDatabaseName, newDatabaseName, fileName)
    {
    }

    private readonly List<KeyValuePair<string?, SqlTable>> _sqlTablesByCategoryOrignal = [];
    private readonly List<KeyValuePair<string?, SqlTable>> _skippedSqlTablesByCategoryOriginal = [];

    //private readonly List<KeyValuePair<string?, SqlTable>> _sqlTablesByCategoryNew = new List<KeyValuePair<string, SqlTable>>();
    //private readonly List<KeyValuePair<string?, SqlTable>> _skippedSqlTablesByCategoryNew = new List<KeyValuePair<string, SqlTable>>();

    public void Document(IDatabaseDefinition originalDd, IDatabaseDefinition newDd)
    {
        Log(LogSeverity.Information, "Starting on {OriginalDatabaseName} vs. {NewDatabaseName}", "ChangeDocumenter", OriginalDatabaseName, NewDatabaseName);

        var tablesOriginal = RemoveKnownTechnicalTables(originalDd.GetTables());

        foreach (var table in tablesOriginal)
        {
            if (!Context.CustomizerOriginal.ShouldSkip(table.SchemaAndTableName!))
                _sqlTablesByCategoryOrignal.Add(new KeyValuePair<string?, SqlTable>(Context.CustomizerOriginal.Category(table.SchemaAndTableName!), table));
            else
                _skippedSqlTablesByCategoryOriginal.Add(new KeyValuePair<string?, SqlTable>(Context.CustomizerOriginal.Category(table.SchemaAndTableName!), table));
        }

        var tablesNew = RemoveKnownTechnicalTables(newDd.GetTables());

        foreach (var table in tablesNew)
        {
            if (!Context.CustomizerNew.ShouldSkip(table.SchemaAndTableName!))
                _sqlTablesByCategoryOrignal.Add(new KeyValuePair<string?, SqlTable>(Context.CustomizerNew.Category(table.SchemaAndTableName!), table));
            else
                _skippedSqlTablesByCategoryOriginal.Add(new KeyValuePair<string?, SqlTable>(Context.CustomizerNew.Category(table.SchemaAndTableName!), table));
        }

        WriteLine("Database", "", "Original", "New");
        WriteLine("Database", "Database name", OriginalDatabaseName, NewDatabaseName);

        var noOfTablesOriginal = originalDd.GetTables().Count;
        var noOfNotSkippedTablesOriginal = originalDd.GetTables().Count(t => !Context.CustomizerOriginal.ShouldSkip(t.SchemaAndTableName!));
        var noOfTablesNew = newDd.GetTables().Count;
        var noOfNotSkippedTablesNew = newDd.GetTables().Count(t => !Context.CustomizerNew.ShouldSkip(t.SchemaAndTableName!));

        WriteLine("Database", "Number of documented tables", noOfNotSkippedTablesOriginal, noOfNotSkippedTablesNew);
        WriteLine("Database", "Number of skipped tables", noOfTablesOriginal - noOfNotSkippedTablesOriginal, noOfTablesNew - noOfNotSkippedTablesNew);
        WriteLine("Database", "Number of tables", noOfTablesOriginal, noOfTablesNew);

        var comparer = new Comparer();
        var changes = comparer.Compare(originalDd, newDd);

        WriteLine("Tables", "Schema", "Table Name", "Event");

        foreach (var tableDelete in changes.OfType<TableDelete>())
        {
            if (!Context.CustomizerOriginal.ShouldSkip(tableDelete.SchemaAndTableName!))
                WriteLine("Tables", tableDelete.SchemaAndTableName!.Schema, tableDelete.SchemaAndTableName.TableName, "Deleted");
        }

        foreach (var tableNew in changes.OfType<TableNew>())
        {
            if (!Context.CustomizerNew.ShouldSkip(tableNew.SchemaAndTableName!))
                WriteLine("Tables", tableNew.SchemaAndTableName!.Schema, tableNew.SchemaAndTableName.TableName, "Added");
        }

        var processedTables = new List<SchemaAndTableName>();

        foreach (var change in changes.OfType<ColumnMigration>())
        {
            switch (change)
            {
                //"Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column"

                case ColumnNew column:
                    {
                        if (Context.CustomizerNew.ShouldSkip(column.Table.SchemaAndTableName!))
                            continue;

                        ProcessColumnMigration(processedTables, column.SqlColumn, "New");
                        break;
                    }
                case ColumnDelete column:
                    {
                        if (Context.CustomizerOriginal.ShouldSkip(column.Table.SchemaAndTableName!))
                            continue;

                        ProcessColumnMigration(processedTables, column.SqlColumn, "Delete");
                        break;
                    }
                case ColumnChange column:
                    {
                        if (Context.CustomizerNew.ShouldSkip(column.SqlColumnChanged!.Table.SchemaAndTableName!))
                            continue;

                        ProcessColumnMigration(processedTables, column.SqlColumn, "Original");
                        ProcessColumnMigration(processedTables, column.SqlColumnChanged, "Changed to");
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
                            if (Context.CustomizerNew.ShouldSkip(fkNew.ForeignKey.ReferredTable!.SchemaAndTableName!))
                                continue;

                            ProcessForeignKey(processedFKs, fkNew.ForeignKey, "New");
                            break;
                        }
                    case ForeignKeyDelete fkDelete:
                        {
                            if (Context.CustomizerOriginal.ShouldSkip(fkDelete.ForeignKey.ReferredTable!.SchemaAndTableName!))
                                continue;

                            ProcessForeignKey(processedFKs, fkDelete.ForeignKey, "Delete");

                            break;
                        }
                    case ForeignKeyChange fkChange:
                        {
                            if (Context.CustomizerNew.ShouldSkip(fkChange.NewForeignKey.ReferredTable!.SchemaAndTableName!))
                                continue;

                            ProcessForeignKey(processedFKs, fkChange.ForeignKey, "Original");
                            ProcessForeignKey(processedFKs, fkChange.NewForeignKey, "Change to");

                            break;
                        }
                }
            }
        }

        if (!Context.DocumenterSettings.NoIndexes)
        {
            var processedIndexes = new List<SchemaAndTableName>();

            foreach (var change in changes.OfType<IndexMigration>())
            {
                ProcessTable(processedTables, change.Index.SqlTableOrView!); // Ensure table header

                switch (change)
                {
                    case IndexNew indexNew:
                        {
                            if (Context.CustomizerNew.ShouldSkip(indexNew.Index.SqlTableOrView!.SchemaAndTableName!))
                                continue;

                            ProcessIndex(processedIndexes, indexNew.Index, "New");
                            break;
                        }
                    case IndexDelete indexDelete:
                        {
                            if (Context.CustomizerOriginal.ShouldSkip(indexDelete.Index.SqlTableOrView!.SchemaAndTableName!))
                                continue;

                            ProcessIndex(processedIndexes, indexDelete.Index, "Delete");

                            break;
                        }
                    case IndexChange indexChange:
                        {
                            if (Context.CustomizerNew.ShouldSkip(indexChange.NewIndex.SqlTableOrView!.SchemaAndTableName!))
                                continue;

                            ProcessIndex(processedIndexes, indexChange.Index, "Original");
                            ProcessIndex(processedIndexes, indexChange.NewIndex, "Change to");

                            break;
                        }
                }
            }
        }

        if (!Context.DocumenterSettings.NoUniqueConstraints)
        {
            var processedUniqueConsreaints = new List<SchemaAndTableName>();

            foreach (var change in changes.OfType<UniqueConstraintMigration>())
            {
                ProcessTable(processedTables, change.UniqueConstraint.SqlTableOrView!); // Ensure table header

                switch (change)
                {
                    case UniqueConstraintNew ucNew:
                        {
                            if (Context.CustomizerNew.ShouldSkip(ucNew.UniqueConstraint.SqlTableOrView!.SchemaAndTableNameSafe))
                                continue;

                            ProcessUniqueConstraint(processedUniqueConsreaints, ucNew.UniqueConstraint, "New");
                            break;
                        }
                    case UniqueConstraintDelete ucDelete:
                        {
                            if (Context.CustomizerOriginal.ShouldSkip(ucDelete.UniqueConstraint.SqlTableOrView!.SchemaAndTableNameSafe))
                                continue;

                            ProcessUniqueConstraint(processedUniqueConsreaints, ucDelete.UniqueConstraint, "Delete");

                            break;
                        }
                    case UniqueConstraintChange ucChange:
                        {
                            if (Context.CustomizerNew.ShouldSkip(ucChange.NewUniqueConstraint.SqlTableOrView!.SchemaAndTableNameSafe))
                                continue;

                            ProcessUniqueConstraint(processedUniqueConsreaints, ucChange.UniqueConstraint, "Original");
                            ProcessUniqueConstraint(processedUniqueConsreaints, ucChange.NewUniqueConstraint, "Change to");

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

        Log(LogSeverity.Information, "Writing Document file {FileName} to folder {Folder}", "ChangeDocumenter", fileName, path);

        if (!string.IsNullOrEmpty(path))
        {
            fileName = Path.Combine(path, fileName);
        }

        File.WriteAllBytes(fileName, content);
    }

    private void ProcessForeignKey(List<SchemaAndTableName> processedFKs, ForeignKey fk, string firstColumn)
    {
        if (!processedFKs.Contains(fk.SqlTable.SchemaAndTableNameSafe))
        {
            processedFKs.Add(fk.SqlTable.SchemaAndTableName!);

            var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

            WriteLine(fk.SqlTable.SchemaAndTableName!);

            WriteAndMerge(fk.SqlTable.SchemaAndTableName!, mergeAmount, "Foreign keys");
            WriteLine(fk.SqlTable.SchemaAndTableName!);

            // TODO allow nulls. Check / other properties?
            WriteLine(fk.SqlTable.SchemaAndTableName!, "Event", "Foreign key name", "Column", "Referenced Table", "link", "Referenced Column", "Properties");
        }

        AddForeignKey(fk, firstColumn);
    }

    private void ProcessIndex(List<SchemaAndTableName> procssedIndexes, Index index, string firstColumn)
    {
        if (!procssedIndexes.Contains(index.SqlTableOrView!.SchemaAndTableNameSafe))
        {
            procssedIndexes.Add(index.SqlTableOrView.SchemaAndTableName!);

            var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

            WriteLine(index.SqlTableOrView.SchemaAndTableName!);

            WriteAndMerge(index.SqlTableOrView!.SchemaAndTableName!, mergeAmount, "Indexes");
            WriteLine(index.SqlTableOrView.SchemaAndTableName!);

            WriteLine(index.SqlTableOrView!.SchemaAndTableName!, "Event", "Index name", "Column", "Order", "Include");
        }

        AddIndex(index, firstColumn);
    }

    private void ProcessUniqueConstraint(List<SchemaAndTableName> procssedUniqueConstraints, UniqueConstraint uniqueConstraint, string firstColumn)
    {
        if (!procssedUniqueConstraints.Contains(uniqueConstraint.SqlTableOrView!.SchemaAndTableName!))
        {
            procssedUniqueConstraints.Add(uniqueConstraint.SqlTableOrView.SchemaAndTableName!);

            var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

            WriteLine(uniqueConstraint.SqlTableOrView.SchemaAndTableName!);

            WriteAndMerge(uniqueConstraint.SqlTableOrView.SchemaAndTableName!, mergeAmount, "Unique constraints");
            WriteLine(uniqueConstraint.SqlTableOrView.SchemaAndTableName!);

            WriteLine(uniqueConstraint.SqlTableOrView.SchemaAndTableName!, "Unique constraint name", "Column");
        }

        AddUniqueConstraint(uniqueConstraint, firstColumn);
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
        // TODO SqlTable and SqlView
        if (!processedTables.Contains(table.SchemaAndTableNameSafe))
        {
            processedTables.Add(table.SchemaAndTableNameSafe);
            // TODO category
            AddTableHeader(false, null, table, "Event");
        }
    }

    protected override Color? GetColor(SchemaAndTableName schemaAndTableName)
    {
        // TODO coloring to incude schema
        var hexColor = Context.CustomizerNew.BackGroundColor(schemaAndTableName);

        if (hexColor is null)
            return null;

        return ColorTranslator.FromHtml(hexColor);
    }
}
