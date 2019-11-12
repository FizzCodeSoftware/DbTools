namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public abstract class DocumenterBase
    {
        protected DocumenterContext Context { get; }

        protected DocumenterHelper Helper { get; set; }

        protected string DatabaseName { get; }

        protected DocumenterBase(DocumenterContext context, string databaseName = "")
        {
            DatabaseName = databaseName;
            Helper = new DocumenterHelper(context.Settings);
            Context = context;
        }
    }
}
