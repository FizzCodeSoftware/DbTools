namespace FizzCode.DbTools.DataDefinitionDocumenter;

public class ChangeDocumenterContext : DocumenterContext
{
    public required ITableCustomizer CustomizerOriginal { get; init; }
    public required ITableCustomizer CustomizerNew { get; init; }
}
