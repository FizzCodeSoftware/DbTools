namespace FizzCode.DbTools.QueryBuilder;

public class JoinSubQueryOn(Query query, string? alias, Expression on, JoinType joinType, params QueryColumn[] columns)
    : JoinOn(query.Table, null, on, joinType, columns)
{
    public Query SubQuery { get; set; } = query;

    public string? Alias { get; set; } = alias;

    public override string ToString()
    {
#pragma warning disable IDE0071 // Simplify interpolation
        return $"{JoinType.ToString()}Join subquery {Table.SchemaAndTableName} AS {Alias}";
#pragma warning restore IDE0071 // Simplify interpolation
    }
}
