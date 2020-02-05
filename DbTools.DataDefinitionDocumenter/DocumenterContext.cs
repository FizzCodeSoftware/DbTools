namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.Common;

    public class DocumenterContext : Context
    {
        public DocumenterSettings DocumenterSettings { get; set; }
        public ITableCustomizer Customizer { get; set; }
    }
}
