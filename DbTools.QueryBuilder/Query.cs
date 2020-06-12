namespace FizzCode.DbTools.QueryBuilder
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition;

    public class Query : QueryElement
    {
        public Query(SqlTable table, string alias = null, params QueryColumn[] columns)
            : base(table, alias, columns)
        {
            QueryElements.Add(this);
        }

        public Query(SqlTable table, params QueryColumn[] columns)
            : this(table, null, columns)
        {
        }

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

        public Query Join(SqlTable table, params QueryColumn[] columns)
        {
            return Join(table, null, columns);
        }

        public Query Join(SqlTable table, string alias, params QueryColumn[] columns)
        {
            return Join(table, null, null, alias, columns);
        }

        public Query Join(SqlTable table, QueryColumn columnTo, QueryColumn columnFrom, string alias, params QueryColumn[] columns)
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

        public Query JoinInner(SqlTable table, params QueryColumn[] columns)
        {
            return JoinInner(table, null, columns);
        }

        public Query JoinInner(SqlTable table, string alias, params QueryColumn[] columns)
        {
            return Join(new Join(table, null, null, alias, JoinType.Inner, columns));
        }

        public Query JoinOn(SqlTable table, string alias, Expression on, params QueryColumn[] columns)
        {
            return Join(new JoinOn(table, on, alias, JoinType.Left, columns));
        }

        public Query JoinOnInner(SqlTable table, string alias, Expression on, params QueryColumn[] columns)
        {
            return Join(new JoinOn(table, on, alias, JoinType.Inner, columns));
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

        private object GetExpression(object[] expressionParts)
        {
            return Expression.GetExpression(expressionParts, QueryElements);
        }
    }
}
