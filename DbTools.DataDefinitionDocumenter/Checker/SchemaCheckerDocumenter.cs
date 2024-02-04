using System.IO;
using System.Linq;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Checker;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class SchemaCheckerDocumenter : DocumenterWriterBase
{
    public SchemaCheckerDocumenter(DocumenterContext context, SqlEngineVersion version, string databaseName = "", string fileName = null)
        : this(new DocumenterWriterExcel(), context, version, databaseName, fileName)
    {
    }

    protected SchemaCheckerDocumenter(IDocumenterWriter documenterWriter, DocumenterContext context, SqlEngineVersion version, string databaseName = "", string fileName = null)
        : base(documenterWriter, context, version, databaseName, fileName)
    {
    }

    public void Document(IDatabaseDefinition dd)
    {
        Log(LogSeverity.Information, "Starting on {DatabaseName}.", "SchemaCheckerDocumenter", DatabaseName);

        WriteLine("Checks", "Severity", "Type", "Target", "Schema", "Table", "Case", "Description");

        var schemaChecker = new SchemaChecker(Context);

        var results = schemaChecker.Check(dd);

        foreach (var schemaCheck in results.OrderBy(sc => sc.Schema).ThenBy(sc => sc.ElementName).ThenByDescending(sc => sc.Severity))
        {
            WriteLine("Checks", schemaCheck.Severity, schemaCheck.Type, schemaCheck.TargetType, schemaCheck.Schema, schemaCheck.ElementName, schemaCheck.DisplayName, schemaCheck.DisplayInfo);
        }

        Log(LogSeverity.Information, "Generating Document content.", "SchemaCheckerDocumenter");
        var content = DocumenterWriter.GetContent();

        var fileName = FileName ?? (DatabaseName?.Length == 0 ? "Database_checks.xlsx" : DatabaseName + "_checks.xlsx");

        var path = Context.DocumenterSettings?.WorkingDirectory;

        Log(LogSeverity.Information, "Writing Document file {FileName} to folder {Folder}", "SchemaCheckerDocumenter", fileName, path);

        if (!string.IsNullOrEmpty(path))
        {
            fileName = Path.Combine(path, fileName);
        }

        File.WriteAllBytes(fileName, content);
    }
}
