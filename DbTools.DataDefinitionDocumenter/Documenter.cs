using System.Collections.Generic;
using System.IO;
using System.Linq;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public partial class Documenter(IDocumenterWriter documenterWriter, DocumenterContext context, SqlEngineVersion version, string databaseName = "", string? fileName = null)
    : DocumenterWriterBase(documenterWriter, context, version, databaseName, fileName)
{
    public Documenter(DocumenterContext context, SqlEngineVersion version, string databaseName = "", string? fileName = null)
        : this(new DocumenterWriterExcel(), context, version, databaseName, fileName)
    {
    }

    private ITableCustomizer? Customizer { get; } = context.Customizer is PatternMatchingTableCustomizer customizer
            ? new PatternMatchingTableCustomizerWithTablesAndItems(customizer)
            : context.Customizer;
    public void Document(IDatabaseDefinition databaseDefinition)
    {
        Log(LogSeverity.Information, "Starting on {DatabaseName}.", "Documenter", DatabaseName);

        var tables = RemoveKnownTechnicalTables(databaseDefinition.GetTables());
        var views = databaseDefinition.GetViews();

        var _sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();
        var _skippedSqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();

        var _sqlViewsByCategory = new List<KeyValuePair<string, SqlView>>();
        var _skippedSqlViewsByCategory = new List<KeyValuePair<string, SqlView>>();

        foreach (var table in tables)
        {
            var schemaAndTableName = table.SchemaAndTableNameSafe;

            if (Customizer is not null && !Customizer.ShouldSkip(schemaAndTableName))
                _sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(Customizer.Category(schemaAndTableName), table));
            else
                _skippedSqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(Customizer.Category(table.SchemaAndTableName), table));
        }

        foreach (var view in views)
        {
            if (!Customizer.ShouldSkip(view.SchemaAndTableName))
                _sqlViewsByCategory.Add(new KeyValuePair<string, SqlView>(Customizer.Category(view.SchemaAndTableName), view));
            else
                _skippedSqlViewsByCategory.Add(new KeyValuePair<string, SqlView>(Customizer.Category(view.SchemaAndTableName), view));
        }

        var hasCategories = _sqlTablesByCategory.Any(x => !string.IsNullOrEmpty(x.Key));

        var noOfTables = databaseDefinition.GetTables().Count;
        var noOfNotSkippedTables = databaseDefinition.GetTables().Count(t => !Customizer.ShouldSkip(t.SchemaAndTableName));
        var noOfNotSkippedViews = databaseDefinition.GetViews().Count(t => !Customizer.ShouldSkip(t.SchemaAndTableName));

        WriteLine("Database", "Database name", DatabaseName);
        WriteLine("Database", "Number of documented tables", noOfNotSkippedTables);
        WriteLine("Database", "Number of documented views", noOfNotSkippedViews);
        WriteLine("Database", "Number of skipped tables", noOfTables - noOfNotSkippedTables);
        WriteLine("Database", "Number of tables", noOfTables);

        if (hasCategories)
        {
            WriteLine("Database");
            WriteLine("Database", "Documented category", "Table count");

            Context.Logger.Log(LogSeverity.Verbose, "Writing tables by category.", "Documenter");

            foreach (var category in _sqlTablesByCategory.Select(kvp => kvp.Key).Distinct().OrderBy(x => x))
            {
                WriteLine("Database", category ?? "(No category)", _sqlTablesByCategory.Count(kvp => kvp.Key == category));
            }

            if (_skippedSqlTablesByCategory.Count > 0)
            {
                WriteLine("Database");
                WriteLine("Database", "Skipped category", "Table count");

                foreach (var category in _skippedSqlTablesByCategory.Select(kvp => kvp.Key).Distinct().OrderBy(x => x))
                {
                    WriteLine("Database", category ?? "(No category)", _skippedSqlTablesByCategory.Count(kvp => kvp.Key == category));
                }
            }
        }
        WriteTablesAndAllColumnsHeaderLine(hasCategories);

        WriteViewsAndAllColumnsHeaderLine(hasCategories);

        // Ensure sheet order
        if (Customizer is PatternMatchingTableCustomizer)
        {
            Write("Patt.ma.-tables");
            Write("Patt.ma.-patterns");
            Write("Patt.ma.-ma.s w exceptions");
            Write("Patt.ma.-no matches (unused)");
        }

        AddTables(_sqlTablesByCategory, _skippedSqlTablesByCategory, hasCategories);

        AddViews(_sqlViewsByCategory, _skippedSqlViewsByCategory, hasCategories);

        Context.Logger.Log(LogSeverity.Verbose, "Generating pattern matching info.", "Documenter");
        AddPatternMatching();
        AddPatternMatchingNoMatch();

        Log(LogSeverity.Information, "Generating Document content.", "Documenter");
        var content = DocumenterWriter.GetContent();

        var fileName = FileName ?? (DatabaseName?.Length == 0 ? "Database.xlsx" : DatabaseName + ".xlsx");

        var path = Context.DocumenterSettings?.WorkingDirectory;

        Log(LogSeverity.Information, "Writing Document file {FileName} to folder {Folder}", "Documenter", fileName, path);

        if (!string.IsNullOrEmpty(path))
        {
            fileName = Path.Combine(path, fileName);
        }

        File.WriteAllBytes(fileName, content);
    }

    protected void AddTableToTableList(string category, SqlTable table, bool hasCategories)
    {
        if (hasCategories)
        {
            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", category);
        }

        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.SchemaAndTableName.Schema);
        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.SchemaAndTableName.TableName);

        if (!Customizer.ShouldSkip(table.SchemaAndTableName))
            DocumenterWriter.WriteLink("Tables", "link", Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName), GetColor(table.SchemaAndTableName));
        else
            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", "");

        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.Columns.Count);

        var tableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();
        if (tableDescription != null)
            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", tableDescription.Description);

        DocumenterWriter.WriteLine("Tables");
    }

    protected void AddViewToViewList(string category, SqlView view, bool hasCategories)
    {
        if (hasCategories)
        {
            DocumenterWriter.Write(GetColor(view.SchemaAndTableName), "Views", category);
        }

        DocumenterWriter.Write(GetColor(view.SchemaAndTableName), "Views", view.SchemaAndTableName.Schema);
        DocumenterWriter.Write(GetColor(view.SchemaAndTableName), "Views", view.SchemaAndTableName.TableName);

        if (!Customizer.ShouldSkip(view.SchemaAndTableName))
            DocumenterWriter.WriteLink("Views", "link", Helper.GetSimplifiedSchemaAndTableName(view.SchemaAndTableName), GetColor(view.SchemaAndTableName));
        else
            DocumenterWriter.Write(GetColor(view.SchemaAndTableName), "Views", "");

        DocumenterWriter.Write(GetColor(view.SchemaAndTableName), "Views", view.Columns.Count);

        // TODO SqlViewDescription
        /*
        var tableDescription = view.Properties.OfType<>(SqlViewDescription).FirstOrDefault();
        if (tableDescription != null)
            DocumenterWriter.Write(GetColor(view.SchemaAndTableName), "Views", tableDescription.Description);
        */

        DocumenterWriter.WriteLine("Views");
    }
}
