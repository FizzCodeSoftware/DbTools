using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public class Join(SqlTable table, string? alias, QueryColumn? columnSource, QueryColumn? columnTarget, JoinType joinType, params QueryColumn[] columns)
    : JoinBase(table, alias, joinType, columns)
{
    public QueryColumn? ColumnSource { get; set; } = columnSource;
    public QueryColumn? ColumnTarget { get; set; } = columnTarget;
}
