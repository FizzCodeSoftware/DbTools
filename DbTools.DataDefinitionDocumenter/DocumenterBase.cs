namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.Common;

    public abstract class DocumenterBase
    {
        protected readonly string _databaseName;
        protected readonly ITableCustomizer _tableCustomizer;
        protected DocumenterHelper Helper { get; set; }

        protected DocumenterSettings DocumenterSettings { get; }

        protected DocumenterBase(DocumenterSettings documenterSettings, Settings settings, string databaseName = "", ITableCustomizer tableCustomizer = null)
        {
            _databaseName = databaseName;
            _tableCustomizer = tableCustomizer ?? new EmptyTableCustomizer();
            Helper = new DocumenterHelper(settings);
            DocumenterSettings = documenterSettings;
        }
    }
}
