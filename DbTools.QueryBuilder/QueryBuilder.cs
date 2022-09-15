namespace FizzCode.DbTools.QueryBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.QueryBuilder.Interface;

    public class QueryBuilder : IQueryBuilderConnector
    {
        public void ProcessStoredProcedureFromQuery(IStoredProcedureFromQuery storedProcedureFromQuery)
        {
            var sp = storedProcedureFromQuery as StoredProcedureFromQuery;

            foreach (var p in GetParametersFromFilters(sp.Query))
                sp.SpParameters.Add(p);

            sp.SqlStatementBody = Build(sp.Query);
        }

        public void ProcessViewFromQuery(IViewFromQuery viewFromQuery)
        {
            var view = viewFromQuery as ViewFromQuery;
            view.SqlStatementBody = Build(view.Query);
        }

        private Query _query;
        private int _level;

        public string Build(Query query)
        {
            return Build(query, 0);
        }

        protected string Build(Query query, int level)
        {
            _query = query;
            _level = level;

            var sb = new StringBuilder();

            sb.Append("SELECT ");
            if (_query.IsDisctinct)
                sb.Append("DISTINCT ");

            sb.Append(AddQueryElementColumns(_query));
            sb.Append(AddJoinColumns());
            sb.AppendLine();
            sb.Append(_level, "FROM ");
            sb.Append(QueryHelper.GetSimplifiedSchemaAndTableName(_query.Table.SchemaAndTableName));
            sb.Append(' ');
            sb.Append(_query.Table.GetAlias());

            sb.Append(AddJoins());

            sb.Append(AddWhere());
            sb.Append(AddFilters());
            sb.Append(AddGroupBy(_query));

            foreach (var union in _query.Unions)
            {
                sb.AppendLine();
                sb.AppendLine(_level, "UNION");
                sb.Append(Build(union));
            }

            return sb.ToString();
        }

        private string AddQueryElementColumns(QueryElement queryElement, bool useAlias = false)
        {
            var columns = queryElement.GetColumns();

            if (columns == null)
                return "";

            var sb = new StringBuilder();

            var last = columns.LastOrDefault();
            foreach (var column in columns)
            {
                if (column.Alias != null)
                {
                    sb.Append(column.Alias);
                    sb.Append('.');
                }
                else if (column.IsDbColumn)
                {
                    sb.Append(queryElement.Table.GetAlias());
                    sb.Append('.');
                }

                sb.Append(column.Value);

                if (column.As != null)
                {
                    sb.Append(" AS '");
                    sb.Append(column.As);
                    sb.Append('\'');
                }
                else if (useAlias)
                {
                    if (_query.QueryColumnAliasStrategy != QueryColumnAliasStrategy.EnableDuplicates)
                    {
                        if (_query.QueryColumnAliasStrategy == QueryColumnAliasStrategy.PrefixTableAliasAlways)
                        {
                            sb.Append(" AS '");
                            sb.Append(queryElement.Table.GetAlias());
                            sb.Append('_');
                            sb.Append(column.Value);
                            sb.Append('\'');
                        }
                        else if (_query.QueryColumnAliasStrategy == QueryColumnAliasStrategy.PrefixTableNameIfNeeded
                             || _query.QueryColumnAliasStrategy == QueryColumnAliasStrategy.PrefixTableAliasIfNeeded)
                        {
                            var hasColumnWithSameName = HasColumnWithSameName(_query, column);
                            if (!hasColumnWithSameName)
                            {
                                foreach (var qe in _query.QueryElements.Where(qe => qe != queryElement))
                                {
                                    hasColumnWithSameName = HasColumnWithSameName(qe, column);
                                    if (hasColumnWithSameName)
                                        break;
                                }
                            }

                            if (hasColumnWithSameName)
                            {
                                sb.Append(" AS '");
                                if (_query.QueryColumnAliasStrategy == QueryColumnAliasStrategy.PrefixTableNameIfNeeded)
                                {
                                    sb.Append(QueryHelper.GetSimplifiedSchemaAndTableName(queryElement.Table.SchemaAndTableName));
                                }
                                else // PrefixTableAliasIfNeeded
                                {
                                    sb.Append(queryElement.Table.GetAlias());
                                    sb.Append('_');
                                }

                                sb.Append(column.Value);
                                sb.Append('\'');
                            }
                        }
                        else if (_query.QueryColumnAliasStrategy == QueryColumnAliasStrategy.PrefixTableNameAlways)
                        {
                            sb.Append(" AS '");
                            sb.Append(QueryHelper.GetSimplifiedSchemaAndTableName(queryElement.Table.SchemaAndTableName));
                            sb.Append(column.Value);
                            sb.Append('\'');
                        }
                        else
                        {
#pragma warning disable IDE0071 // Simplify interpolation
                            throw new NotImplementedException($"Unhandled QueryColumnAliasStrategy {_query.QueryColumnAliasStrategy.ToString()}.");
#pragma warning restore IDE0071 // Simplify interpolation
                        }
                    }
                }

                if (column != last)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        private bool HasColumnWithSameName(QueryElement queryElement, QueryColumn queryColumn)
        {
            var columns = queryElement.GetColumns();
            if (columns == null)
                return false;

            return columns.Any(c => c.Value == queryColumn.Value);
        }

        private string AddJoinColumns()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < _query.Joins.Count; i++)
            {
                if (i == 0 && _query.QueryColumns.Count == 1 && _query.QueryColumns[0] is None)
                    sb.Append(AddQueryElementColumns(_query.Joins[i], true));
                else
                    sb.AppendComma(AddQueryElementColumns(_query.Joins[i], true));
            }

            return sb.ToString();
        }

        private string AddJoins()
        {
            var sb = new StringBuilder();

            foreach (var join in _query.Joins)
                sb.Append(AddJoin(join));

            return sb.ToString();
        }

        private string AddJoin(JoinBase join)
        {
            var sb = new StringBuilder();

            var joinType = join.JoinType.ToString().ToUpperInvariant();
            sb.AppendLine()
                .Append(_level, joinType)
                .Append(" JOIN");

            if (join is JoinSubQueryOn joinSubQueryOn)
            {
                var subQb = new QueryBuilder();
                sb.AppendLine()
                    .Append(_level + 1, "(")
                    .Append(subQb.Build(joinSubQueryOn.SubQuery, _level + 1))
                    .Append(") ")
                    .Append(joinSubQueryOn.Alias);
            }
            else
            {
                sb.Append(' ')
                    .Append(QueryHelper.GetSimplifiedSchemaAndTableName(join.Table.SchemaAndTableName))
                    .Append(' ')
                    .Append(join.Table.GetAlias());
            }
            sb.Append(" ON ");

            if (join is Join join2)
                sb.Append(AddJoin(join2));

            if (join is JoinOn joinOn)
                sb.Append(AddJoinOn(joinOn));

            return sb.ToString();
        }

        private string AddJoinOn(JoinOn joinOn)
        {
            return Expression.GetExpression(joinOn.OnExpression, _query.QueryElements, joinOn);
        }

        private string AddJoin(Join join)
        {
            var sb = new StringBuilder();

            var queryTableProperties = (_query.Table as SqlTable).Properties;

            if (join.ColumnSource == null && join.ColumnTarget == null)
            { // auto build JOIN ON
                var fk = queryTableProperties.OfType<ForeignKey>().First(fk => fk.ForeignKeyColumns[0].ReferredColumn.Table.SchemaAndTableName == join.Table.SchemaAndTableName);

                foreach (var fkm in fk.ForeignKeyColumns)
                {
                    sb.Append(join.Table.GetAlias());
                    sb.Append('.');
                    sb.Append(fkm.ReferredColumn.Name);
                    sb.Append(" = ");
                    sb.Append(_query.Table.GetAlias());
                    sb.Append('.');
                    sb.Append(fkm.ForeignKeyColumn.Name);
                }
            }
            else if (join.ColumnSource != null && join.ColumnTarget != null)
            {
                sb.Append(join.Table.GetAlias());
                sb.Append('.');
                sb.Append(join.ColumnSource.Value);
                sb.Append(" = ");
                sb.Append(_query.Table.GetAlias());
                sb.Append('.');
                sb.Append(join.ColumnTarget.Value);
            }
            else if (join.ColumnSource == null && join.ColumnTarget != null)
            {
                var joinTableProperties = (join.Table as SqlTable).Properties;
                var pk = joinTableProperties.OfType<PrimaryKey>().FirstOrDefault();
                if (pk == null)
                    throw new ArgumentException($"Target Join table has no Primary Key. Table: {join.Table.SchemaAndTableName}, source column: {join.ColumnSource}.");

                if (pk.SqlColumns.Count > 1)
                    throw new ArgumentException($"Target Join table Primary Key with multiple columns, Table: {join.Table.SchemaAndTableName}, source column: {join.ColumnSource}.");

                var cao = pk.SqlColumns[0];

                sb.Append(join.Table.GetAlias());
                sb.Append('.');
                sb.Append(cao.SqlColumn.Name);
                sb.Append(" = ");
                sb.Append(_query.Table.GetAlias());
                sb.Append('.');
                sb.Append(join.ColumnTarget.Value);
            }
            else if (join.ColumnSource != null && join.ColumnTarget == null)
            {
                var pk = queryTableProperties.OfType<PrimaryKey>().FirstOrDefault();
                if (pk == null)
                    throw new ArgumentException($"Target Join table has no Primary Key. Table: {join.Table.SchemaAndTableName}, source column: {join.ColumnSource}.");

                if (pk.SqlColumns.Count > 1)
                    throw new ArgumentException($"Target Join table Primary Key with multiple columns, Table: {join.Table.SchemaAndTableName}, source column: {join.ColumnSource}.");

                var cao = pk.SqlColumns[0];

                sb.Append(join.Table.GetAlias());
                sb.Append('.');
                sb.Append(join.ColumnSource.Value);
                sb.Append(" = ");
                sb.Append(_query.Table.GetAlias());
                sb.Append('.');
                sb.Append(cao.SqlColumn.Name);
            }

            return sb.ToString();
        }

        private string AddWhere()
        {
            if (!string.IsNullOrEmpty(_query.WhereExpression))
                return "\r\n" + StringBuilderExtensions.Spaces(_level) + "WHERE " + _query.WhereExpression;

            return null;
        }

        private string AddGroupBy(QueryElement queryElement)
        {
            if (_query.GroupByColumns.Count == 0)
                return null;

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.Append(_level, "GROUP BY ");

            var last = _query.GroupByColumns.Last();
            foreach (var column in _query.GroupByColumns)
            {
                if (column.Alias != null)
                {
                    sb.Append(column.Alias);
                    sb.Append('.');
                }
                else if (column.IsDbColumn)
                {
                    sb.Append(queryElement.Table.GetAlias());
                    sb.Append('.');
                }

                sb.Append(column.Value);

                if (column != last)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        private string AddFilters()
        {
            if (_query.Filters.Count == 0)
                return null;

            var sb = new StringBuilder();

            sb.AppendLine("\r\n");

            if (string.IsNullOrEmpty(_query.WhereExpression))
                sb.Append(_level, "WHERE ");

            foreach (var filter in _query.Filters)
            {
                switch (filter.Type)
                {
                    case FilterType.Between:
                        //(@minStartDate IS NULL OR p.StartDate >= @minStartDate) AND (@maxStartDate IS NULL OR p.StartDate <= @maxStartDate)
                        // TODO Alias for column
                        sb.Append("(@min").Append(filter.Column.Value).Append(" IS NULL OR ").Append(filter.Column.Alias).Append('.').Append(filter.Column.Value).Append(" >= @min").Append(filter.Column.Value).Append(") AND (@max").Append(filter.Column.Value).Append(" IS NULL OR ").Append(filter.Column.Alias).Append('.').Append(filter.Column.Value).Append(" <= @max").Append(filter.Column.Value).Append(')');
                        break;
                    default:
#pragma warning disable IDE0071 // Simplify interpolation
                        throw new NotImplementedException($"Filtering for {filter.Type.ToString()} is not implemented.");
#pragma warning restore IDE0071 // Simplify interpolation
                }
            }

            return sb.ToString();
        }

        public List<SqlParameter> GetParametersFromFilters(Query query)
        {
            var parameters = new List<SqlParameter>();

            foreach (var filter in query.Filters)
            {
                switch (filter.Type)
                {
                    case FilterType.Between:
                        {
                            parameters.Add(filter.Parameter.Copy("min" + filter.Column.Value));
                            parameters.Add(filter.Parameter.Copy("max" + filter.Column.Value));
                            break;
                        }

                    case FilterType.Equal:
                    case FilterType.Greater:
                    case FilterType.Lesser:
                        parameters.Add(filter.Parameter);
                        break;
                    default:
#pragma warning disable IDE0071 // Simplify interpolation
                        throw new NotImplementedException($"Filtering for {filter.Type.ToString()} is not implemented.");
#pragma warning restore IDE0071 // Simplify interpolation
                }
            }

            return parameters;
        }
    }
}
