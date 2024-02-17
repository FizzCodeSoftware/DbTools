namespace FizzCode.DbTools.DataDefinition.Base;
public abstract class IndexBase<T> : SqlTableOrViewPropertyBase<T> where T : SqlTableOrView
{
    public string? Name { get; set; }


    public List<ColumnAndOrderRegistration> SqlColumnRegistrations { get; set; } = [];
    public List<ColumnAndOrder> SqlColumns { get; set; } = [];

    public bool Unique { get; set; }
    public bool? Clustered { get; init; }

    protected IndexBase(T sqlTable, string? name, bool unique = false)
        : base(sqlTable)
    {
        Name = name;
        Unique = unique;
    }

    protected string GetColumnsInString(bool withOrder = false)
    {
        if (withOrder)
            return string.Join(", ", SqlColumns);

        return string.Join(", ", SqlColumns.Select(cao => cao.SqlColumn.Name));
    }
}
