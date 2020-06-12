namespace FizzCode.DbTools.QueryBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public class QueryBuilder
    {
        private Query _query;

        public string Build(Query query)
        {
            _query = query;

            var sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append(AddQueryElementColumns(_query));
            sb.Append(AddJoinColumns());
            sb.Append("\r\nFROM ");
            sb.Append(QueryHelper.GetSimplifiedSchemaAndTableName(query.Table.SchemaAndTableName));
            sb.Append(" ");
            sb.Append(query.Alias);

            sb.Append(AddJoins(query));

            sb.Append(AddWhere(query));

            foreach (var union in query.Unions)
            {
                sb.AppendLine();
                sb.AppendLine("UNION");
                sb.Append(Build(union));
            }

            return sb.ToString();
        }

        private static string AddQueryElementColumns(QueryElement queryElement, bool useAlias = false)
        {
            // TODO prefix same column names
            var sb = new StringBuilder();

            if (queryElement.QueryColumns.Count == 1
                && queryElement.QueryColumns[0] is None)
            {
                return "";
            }

            var columns = new List<QueryColumn>();

            columns = queryElement.QueryColumns.Count == 0
                ? queryElement.Table.Columns.Select(c => (QueryColumn)c).ToList()
                : queryElement.QueryColumns;

            var last = columns.LastOrDefault();
            foreach (var column in columns)
            {
                if (column.IsDbColumn)
                {
                    sb.Append(queryElement.Alias);
                    sb.Append(".");
                }

                sb.Append(column.Value);

                if (column.As != null)
                {
                    sb.Append(" AS '");
                    sb.Append(column.As);
                    sb.Append("'");
                }
                else if (useAlias)
                {
                    sb.Append(" AS '");
                    sb.Append(queryElement.Alias);
                    sb.Append("_");
                    sb.Append(column.Value);
                    sb.Append("'");
                }

                if (column != last)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        private string AddJoinColumns()
        {
            var sb = new StringBuilder();

            foreach (var join in _query.Joins)
                sb.AppendComma(AddQueryElementColumns(join, true));

            return sb.ToString();
        }

        private string AddJoins(Query query)
        {
            var sb = new StringBuilder();

            foreach (var join in _query.Joins)
                sb.Append(AddJoin(query, join));

            return sb.ToString();
        }

        private static string AddJoin(Query query, JoinBase join)
        {
            var sb = new StringBuilder();

            var joinType = join.JoinType.ToString().ToUpperInvariant();
            sb.Append("\r\n")
                .Append(joinType)
                .Append(" JOIN ")
                .Append(QueryHelper.GetSimplifiedSchemaAndTableName(join.Table.SchemaAndTableName))
                .Append(" ")
                .Append(join.Alias)
                .Append(" ON ");

            if(join is Join join2)
                sb.Append(AddJoinOn(query, join2));

            if (join is JoinOn joinOn)
                sb.Append(AddJoinOn(query, joinOn));

            return sb.ToString();
        }

        private static string AddJoinOn(Query query, JoinOn joinOn)
        {
            return Expression.GetExpression(joinOn.OnExpression, query.QueryElements, joinOn);
        }

        private static string AddJoinOn(Query query, Join join)
        {
            var sb = new StringBuilder();

            if (join.ColumnTo == null && join.ColumnFrom == null)
            { // auto build JOIN ON
                var fk = query.Table.Properties.OfType<ForeignKey>().First(fk => fk.ForeignKeyColumns[0].ReferredColumn.Table.SchemaAndTableName == join.Table.SchemaAndTableName);

                foreach (var fkm in fk.ForeignKeyColumns)
                {
                    sb.Append(join.Alias);
                    sb.Append(".");
                    sb.Append(fkm.ReferredColumn.Name);
                    sb.Append(" = ");
                    sb.Append(query.Alias);
                    sb.Append(".");
                    sb.Append(fkm.ForeignKeyColumn.Name);
                }
            }
            else if (join.ColumnTo != null && join.ColumnFrom != null)
            {
                sb.Append(join.Alias);
                sb.Append(".");
                sb.Append(join.ColumnTo.Value);
                sb.Append(" = ");
                sb.Append(query.Alias);
                sb.Append(".");
                sb.Append(join.ColumnFrom.Value);
            }
            else if (join.ColumnTo != null && join.ColumnFrom == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }

            return sb.ToString();
        }

        private static string AddWhere(Query query)
        {
            if (!string.IsNullOrEmpty(query.WhereExpression))
                return "\r\nWHERE " + query.WhereExpression;

            return null;
        }
    }

    public class On
    {
        public string Value { get; set; }

        public static implicit operator On(string on)
        {
            var result = new On
            {
                Value = on
            };
            return result;
        }

        public static implicit operator string(On on)
        {
            return on.Value;
        }
    }
}
