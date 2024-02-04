using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public abstract class JoinBase : QueryElement
{
    protected JoinBase(SqlTableOrView table, string alias, JoinType joinType, params QueryColumn[] columns)
        : base(table, alias, columns)
    {
        JoinType = joinType;
    }

    public JoinType JoinType { get; set; }

    public override string ToString()
    {
#pragma warning disable IDE0071 // Simplify interpolation
        return $"{JoinType.ToString()}Join {Table.SchemaAndTableName} AS {Table.GetAlias()}";
#pragma warning restore IDE0071 // Simplify interpolation
    }
}
