namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;

    public class DocumenterContext : Context
    {
        public DocumenterSettings DocumenterSettings { get; set; }
        public ITableCustomizer Customizer { get; set; }
    }
}
