namespace FizzCode.DbTools.QueryBuilder
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public class Query : QueryElement
    {
        public Query(SqlTable table, string alias = null, params QueryColumn[] columns)
            : base(table, alias, columns)
        {
            QueryElements.Add(Alias, this);
        }

        public Query(SqlTable table, params QueryColumn[] columns)
            : this(table, null, columns)
        {
        }

        public List<Join> Joins { get; } = new List<Join>();
        private Dictionary<string, QueryElement> QueryElements { get; } = new Dictionary<string, QueryElement>();

        public Query Join(Join join)
        {
            Joins.Add(join);
            QueryElements.Add(join.Alias, join);
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

        public string WhereExpression { get; set; }

        public Query Where(params object[] objects)
        {
            var sb = new StringBuilder();
            string previous = null;
            foreach (var obj in objects)
            {
                if (obj is SqlColumn sqlColumn)
                {
                    if (previous?.EndsWith('.') != true)
                    {
                        var table = sqlColumn.Table;
                        var alias = QueryElements.Values.Where(qe => qe.Table == table).Select(qe => qe.Alias).Single();
                        sb.Append(alias);
                        sb.Append(".");
                    }

                    sb.Append(((QueryColumn)sqlColumn).Name);
                    previous = null;
                }

                if (obj is string @string)
                {
                    sb.Append(obj);
                    previous = @string;
                }
            }

            return Where(sb.ToString());
        }

        public Query Where(string whereExpression)
        {
            WhereExpression = whereExpression;
            return this;
        }
    }
}
