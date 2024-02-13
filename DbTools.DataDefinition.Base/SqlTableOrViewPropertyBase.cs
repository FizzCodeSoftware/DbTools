namespace FizzCode.DbTools.DataDefinition.Base;

public abstract class SqlTableOrViewPropertyBase<T> where T : SqlTableOrView
{
    // Nullable for declaration (public SqlTableCustomProperty MyCustomProperty { get; } = new MyCustomProperty();)
    public T? SqlTableOrView { get; set; }

    protected SqlTableOrViewPropertyBase(T? sqlTable)
    {
        SqlTableOrView = sqlTable;
    }

    public SqlEngineVersionSpecificProperties SqlEngineVersionSpecificProperties { get; } = [];
}
