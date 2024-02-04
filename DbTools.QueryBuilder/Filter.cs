using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public class Filter
{
    public SqlTableOrView Table { get; set; }
    public QueryColumn Column { get; set; }
    public FilterType Type { get; set; }

    public SqlParameter Parameter { get; set; }
}

public class FilterExpression
{
}

public enum FilterType
{
    Equal,
    Greater,
    Lesser,
    Between
}
