namespace FizzCode.DbTools.QueryBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public static class StringBuilderExtension
    {
        public static StringBuilder AppendComma(this StringBuilder sb, string value)
        {
            if (!string.IsNullOrEmpty(value))
                sb.Append(", ");

            sb.Append(value);

            return sb;
        }
    }

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
                //if (useAlias)
                //{
                    sb.Append(queryElement.Alias);
                    sb.Append(".");
                //}

                sb.Append(column.Name);

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
                    sb.Append(column.Name);
                    sb.Append("'");
                }

                if(column != last)
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

            foreach(var join in _query.Joins)
                sb.Append(AddJoin(query, join));

            return sb.ToString();
        }

        private string AddJoin(Query query, Join join)
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

            if (join.ColumnTo == null && join.ColumnFrom == null)
            { // auto build JOIN ON
                var fk = _query.Table.Properties.OfType<ForeignKey>().First(fk => fk.ForeignKeyColumns[0].ReferredColumn.Table.SchemaAndTableName == join.Table.SchemaAndTableName);

                foreach (var fkm in fk.ForeignKeyColumns)
                {
                    sb.Append(join.Alias);
                    sb.Append(".");
                    sb.Append(fkm.ReferredColumn.Name);
                    sb.Append(" = ");
                    sb.Append(_query.Alias);
                    sb.Append(".");
                    sb.Append(fkm.ForeignKeyColumn.Name);
                }
            }
            else if (join.ColumnTo != null && join.ColumnFrom != null)
            {
                sb.Append(join.Alias);
                sb.Append(".");
                sb.Append(join.ColumnTo.Name);
                sb.Append(" = ");
                sb.Append(_query.Alias);
                sb.Append(".");
                sb.Append(join.ColumnFrom.Name);
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
    }

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
    }

    public enum JoinType
    {
        Left,
        Inner,
        Right,
        Outer
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

    public class Join : QueryElement
    {
        public Join(SqlTable table, QueryColumn columnTo, QueryColumn columnFrom, string alias, JoinType joinType, params QueryColumn[] columns)
            : base(table, alias, columns)
        {
            ColumnTo = columnTo;
            ColumnFrom = columnFrom;
            JoinType = joinType;
        }
        public QueryColumn ColumnTo { get; set; }
        public QueryColumn ColumnFrom { get; set; }
        public JoinType JoinType { get; set; }
    }

    public class None : QueryColumn
    {
    }

    public class QueryColumn
    {
        public QueryColumn()
        {
        }

        public QueryColumn(QueryColumn column, string alias)
        {
            Name = column.Name;
            As = alias;
        }

        public string Name { get; set; }
        public string As { get; set; }

        public static implicit operator QueryColumn(SqlColumn column)
        {
            var queryColumn = new QueryColumn
            {
                Name = column.Name
            };
            return queryColumn;
        }
    }

    public class QueryElement
    {
        public SqlTable Table { get; set; }
        public string Alias { get; set; }
        public List<QueryColumn> QueryColumns { get; set; }

        public QueryElement(SqlTable table, string alias = null)
        {
            Table = table;
            Alias = alias ?? table.SchemaAndTableName.TableName.Substring(0, 1).ToLowerInvariant();
        }

        public QueryElement(SqlTable table, params QueryColumn[] columns)
            : this(table, null, columns)
        {
        }

        public QueryElement(SqlTable table, string alias, params QueryColumn[] columns)
        {
            Table = table;
            Alias = alias ?? table.SchemaAndTableName.TableName.Substring(0, 1).ToLowerInvariant();
            QueryColumns = columns.ToList();
        }
    }

    public static class QueryHelper
    {
        // TODO temporal for now, duplicated
        public static string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName, string separator = ".")
        {
            var schema = schemaAndTableName.Schema;
            var tableName = schemaAndTableName.TableName;

            //var defaultSchema = Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema", null);
            var defaultSchema = "dbo";

            /*if (Settings.Options.ShouldUseDefaultSchema && schema == null)
                return defaultSchema + separator + tableName;

            if (!Settings.Options.ShouldUseDefaultSchema && schema == defaultSchema)
                return tableName;*/

            if (schema == defaultSchema)
                return tableName;

            if (schema != null)
                return schema + separator + tableName;

            return tableName;
        }
    }

    public static class QueryBuilderHelper
    {
        public static QueryColumn[] Except(this SqlTable table, params SqlColumn[] columns)
        {
            var columnNames = columns.Select(c => c.Name);
            var result = table.Columns.Where(c => !columnNames.Contains(c.Name));
            return result.Select(r => (QueryColumn)r).ToArray();
        }
    }
}
