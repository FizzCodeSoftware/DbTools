namespace FizzCode.DbTools.QueryBuilder
{
    public class JoinSubQueryOn : JoinOn
    {
        public JoinSubQueryOn(Query query, string alias, Expression on, JoinType joinType, params QueryColumn[] columns)
            : base(query.Table, null, on, joinType, columns)
        {
            SubQuery = query;
            Alias = alias;
        }

        public Query SubQuery { get; set; }

        public string Alias { get; set; }

        public override string ToString()
        {
#pragma warning disable IDE0071 // Simplify interpolation
            return $"{JoinType.ToString()}Join subquery {Table.SchemaAndTableName} AS {Alias}";
#pragma warning restore IDE0071 // Simplify interpolation
        }
    }
}
