using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public class Filter
{
    public required SqlTableOrView Table { get; init; }
    public required QueryColumn Column { get; init; }
    public FilterType Type { get; init; }

    public required SqlParameter Parameter { get; init; }
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
