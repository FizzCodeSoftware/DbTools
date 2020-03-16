namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;

    public class ChangeDocumenterContext : DocumenterContext
    {
        private new ITableCustomizer Customizer { get; }

        public ITableCustomizer CustomizerOriginal { get; set; }
        public ITableCustomizer CustomizerNew { get; set; }
    }

    public class ChangeDocumenter : DocumenterWriterBase
    {
        protected string OriginalDatabaseName
        {
            get
            {
                return base.DatabaseName;
            }
        }

        protected string NewDatabaseName { get; }

        private new string DatabaseName { get; }

        protected new ChangeDocumenterContext Context
        {
            get
            {
                return (ChangeDocumenterContext)base.Context;
            }
        }

        public ChangeDocumenter(ChangeDocumenterContext context, Configuration.SqlEngineVersion version, string originalDatabaseName = "", string newDatabaseName = "", string fileName = null)
            : base(context, version, originalDatabaseName, fileName)
        {
            NewDatabaseName = newDatabaseName;
        }

        public ChangeDocumenter(IDocumenterWriter documenterWriter, DocumenterContext context, Configuration.SqlEngineVersion version, string originalDatabaseName = "", string newDatabaseName = "", string fileName = null)
            : base(documenterWriter, context, version, originalDatabaseName, fileName)
        {
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

        }
    }
}
