using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public class JoinOn(SqlTableOrView table, string? alias, Expression on, JoinType joinType, params QueryColumn[] columns)
    : JoinBase(table, alias, joinType, columns)
{
    public Expression OnExpression { get; set; } = on;
}
