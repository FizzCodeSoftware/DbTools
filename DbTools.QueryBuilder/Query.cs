namespace FizzCode.DbTools.QueryBuilder
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition;

    public class Query : QueryElement
    {
        public Query(SqlTable sqlTable, string alias, QueryColumnAliasStrategy queryColumnAliasStrategy, params QueryColumn[] columns)
            : base(sqlTable, alias, columns)
        {
            QueryElements.Add(this);
            QueryColumnAliasStrategy = queryColumnAliasStrategy;
        }

        public Query(SqlTable sqlTable, string alias = null, params QueryColumn[] columns)
            : this(sqlTable, alias, QueryColumnAliasStrategy.PrefixTableNameIfNeeded, columns)
        {
        }

        public Query(SqlTable sqlTable, params QueryColumn[] columns)
            : this(sqlTable, null, columns)
        {
        }

        public Query(SqlTable sqlTable, QueryColumnAliasStrategy queryColumnAliasStrategy, params QueryColumn[] columns)
            : this(sqlTable, null, queryColumnAliasStrategy, columns)
        {
        }

        public QueryColumnAliasStrategy QueryColumnAliasStrategy { get; set; }

        public List<JoinBase> Joins { get; } = new List<JoinBase>();
        public List<Query> Unions { get; } = new List<Query>();
        public List<QueryElement> QueryElements { get; } = new List<QueryElement>();

        public Query Union(Query query)
        {
            Unions.Add(query);
            return this;
        }

        public Query Join(JoinBase join)
        {
            Joins.Add(join);
            QueryElements.Add(join);
            return this;
        }

        public Query LeftJoin(SqlTable table, params QueryColumn[] columns)
        {
            return LeftJoin(table, null, columns);
        }

        public Query LeftJoin(SqlTable table, string alias, params QueryColumn[] columns)
        {
            return LeftJoin(table, null, null, alias, columns);
        }

        public Query LeftJoin(SqlTable table, QueryColumn columnTo, QueryColumn columnFrom, string alias, params QueryColumn[] columns)
        {
            return Join(new Join(table, columnTo, columnFrom, alias, JoinType.Left, columns));
        }

        public Query JoinRight(SqlTable table, params QueryColumn[] columns)
        {
            return JoinRight(table, null, columns);
        }

        public Query JoinRight(SqlTable table, string alias, params QueryColumn[] columns)
        {
            return Join(new Join(table, null, null, alias, JoinType.Right, columns));
        }

        public Query InnerJoin(SqlTable table, params QueryColumn[] columns)
        {
            return InnerJoin(table, null, columns);
        }

        public Query InnerJoin(SqlTable table, string alias, params QueryColumn[] columns)
        {
            return Join(new Join(table, null, null, alias, JoinType.Inner, columns));
        }

        public Query LeftJoinOn(SqlTable table, string alias, Expression on, params QueryColumn[] columns)
        {
            return Join(new JoinOn(table, on, alias, JoinType.Left, columns));
        }

        public Query LeftJoinOn(SqlTable table, Expression on, params QueryColumn[] columns)
        {
            return Join(new JoinOn(table, on, JoinType.Left, columns));
        }

        public Query InnerJoinOn(SqlTable table, string alias, Expression on, params QueryColumn[] columns)
        {
            return Join(new JoinOn(table, on, alias, JoinType.Inner, columns));
        }

        public Query InnerJoinOn(SqlTable table, Expression on, params QueryColumn[] columns)
        {
            return Join(new JoinOn(table, on, JoinType.Inner, columns));
        }

        public string WhereExpression { get; set; }

        public Query Where(params object[] expressionParts)
        {
            return Where(Expression.GetExpression(expressionParts, QueryElements));
        }

        public Query Where(string whereExpression)
        {
            WhereExpression = whereExpression;
            return this;
        }

        public Query AddColumn(string alias, params object[] expressionParts)
        {
            var qc = new QueryColumn
            {
                Value = Expression.GetExpression(expressionParts, QueryElements),
                As = alias
            };

            QueryColumns.Add(qc);

            return this;
        }

        public Query AddCase(string alias, object[] whenExpression, object[] thenExpression, object[] elseExpression)
        {
            var qc = new QueryColumn
            {
                Value = $"CASE WHEN {GetExpression(whenExpression)} THEN {GetExpression(thenExpression)} ELSE {GetExpression(elseExpression)} END",
                As = alias
            };

            QueryColumns.Add(qc);

            return this;
        }

        public bool IsDisctinct { get; set; }

        public Query Disctinct(bool isDistinct = true)
        {
            IsDisctinct = isDistinct;
            return this;
        }

        private object GetExpression(object[] expressionParts)
        {
            return Expression.GetExpression(expressionParts, QueryElements);
        }
    }
}
