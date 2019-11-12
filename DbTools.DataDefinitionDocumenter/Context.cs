namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;

    public class Context
    {
        public DocumenterSettings DocumenterSettings { get; set; }
        public Settings Settings { get; set; }
        public Logger Logger { get; set; }

        public ITableCustomizer Customizer { get; set; }
    }
}
