namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.Common.Logger;

    public abstract class DocumenterBase
    {
        protected DocumenterContextBase Context { get; }

        protected DocumenterHelper Helper { get; set; }

        protected SqlEngineVersion Version { get; set; }

        protected string DatabaseName { get; }

        protected DocumenterBase(DocumenterContextBase context, SqlEngineVersion version, string databaseName = "")
        {
            Context = context;
            Version = version;
            DatabaseName = databaseName;
            Helper = new DocumenterHelper(context.Settings);
        }

        protected void Log(LogSeverity severity, string text, params object[] args)
        {
            Context.Logger.Log(severity, text, args);
        }
    }
}
