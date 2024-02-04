namespace FizzCode.DbTools.DataDefinition.Base;

public abstract class SqlTableOrViewPropertyBase<T> where T : SqlTableOrView
{
    public T SqlTableOrView { get; set; }

    protected SqlTableOrViewPropertyBase(T sqlTable)
    {
        SqlTableOrView = sqlTable;
    }

    public SqlEngineVersionSpecificProperties SqlEngineVersionSpecificProperties { get; } = new SqlEngineVersionSpecificProperties();
}
