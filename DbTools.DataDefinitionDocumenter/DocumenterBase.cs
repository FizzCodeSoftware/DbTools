namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public abstract class DocumenterBase
    {
        protected Context Context { get; }

        protected DocumenterHelper Helper { get; set; }

        protected string DatabaseName { get; }

        protected DocumenterBase(Context context, string databaseName = "")
        {
            DatabaseName = databaseName;
            Helper = new DocumenterHelper(context.Settings);
            Context = context;
        }
    }
}
