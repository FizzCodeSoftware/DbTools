namespace FizzCode.DbTools.DataDefinitionDocumenter
{
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

        public void Document(DatabaseDefinition originalDd, DatabaseDefinition newDd)
        {
            Log(LogSeverity.Information, "Starting on {OriginalDatabaseName} vs. {NewDatabaseName}", "Documenter", OriginalDatabaseName, NewDatabaseName);

            var comparer = new Comparer(Context);
            var changes = comparer.Compare(originalDd, newDd);

            // deleted tables
            // new tables
            // changes

            WriteLine("Database", "", "Original", "New");
            WriteLine("Database", "Database name", OriginalDatabaseName, NewDatabaseName);
            
            var noOfTablesOriginal = originalDd.GetTables().Count;
            var noOfNotSkippedTablesOriginal = originalDd.GetTables().Count(t => !Context.CustomizerOriginal.ShouldSkip(t.SchemaAndTableName));
            var noOfTablesNew = newDd.GetTables().Count;
            var noOfNotSkippedTablesNew = newDd.GetTables().Count(t => !Context.CustomizerNew.ShouldSkip(t.SchemaAndTableName));

            WriteLine("Database", "Number of documented tables", noOfNotSkippedTablesOriginal, noOfNotSkippedTablesNew);
            WriteLine("Database", "Number of skipped tables", noOfTablesOriginal - noOfNotSkippedTablesOriginal, noOfTablesNew - noOfNotSkippedTablesNew);
            WriteLine("Database", "Number of tables", noOfTablesOriginal, noOfTablesNew);

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
    }
}
