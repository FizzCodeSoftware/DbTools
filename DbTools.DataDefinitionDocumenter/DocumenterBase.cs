namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.Common;

    public abstract class DocumenterBase
    {
        protected DocumenterHelper Helper { get; set; }

        protected DocumenterSettings DocumenterSettings { get; }

        protected string DatabaseName { get; }

        protected ITableCustomizer TableCustomizer { get; }

        protected DocumenterBase(DocumenterSettings documenterSettings, Settings settings, string databaseName = "", ITableCustomizer tableCustomizer = null)
        {
            DatabaseName = databaseName;
            TableCustomizer = tableCustomizer ?? new EmptyTableCustomizer();
            Helper = new DocumenterHelper(settings);
            DocumenterSettings = documenterSettings;
        }
    }
}
