namespace FizzCode.DbTools.DataDefinitionDocumenter;

public class ChangeDocumenterContext : DocumenterContext
{
    public ITableCustomizer CustomizerOriginal { get; set; }
    public ITableCustomizer CustomizerNew { get; set; }
}
