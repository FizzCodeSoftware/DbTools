namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;

    public class ChangeDocumenterContext : DocumenterContext
    {
        public ITableCustomizer CustomizerOriginal { get; set; }
        public ITableCustomizer CustomizerNew { get; set; }
    }
}
