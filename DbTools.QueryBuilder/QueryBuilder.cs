namespace FizzCode.DbTools.QueryBuilder
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.QueryBuilder.Interface;

    public class QueryBuilder : IQueryBuilder
    {
        private Query _query;

        public string Build(IQuery query)
        {
            _query = query as Query;

            var sb = new StringBuilder();

            sb.Append("SELECT ");
            if (_query.IsDisctinct)
                sb.Append("DISTINCT ");

            sb.Append(AddQueryElementColumns(_query));
            sb.Append(AddJoinColumns());
            sb.Append("\r\nFROM ");
            sb.Append(QueryHelper.GetSimplifiedSchemaAndTableName(_query.Table.SchemaAndTableName));
            sb.Append(" ");
            sb.Append(_query.Table.GetAlias());

            sb.Append(AddJoins());

            sb.Append(AddWhere());

            foreach (var union in _query.Unions)
            {
                sb.AppendLine();
                sb.AppendLine("UNION");
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
                    sb.Append(".");
                }
                else if (column.IsDbColumn)
                {
                    sb.Append(queryElement.Table.GetAlias());
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
                    if (_query.QueryColumnAliasStrategy != QueryColumnAliasStrategy.EnableDuplicates)
                    {
                        if (_query.QueryColumnAliasStrategy == QueryColumnAliasStrategy.PrefixTableAliasAlways)
                        {
                            sb.Append(" AS '");
                            sb.Append(queryElement.Table.GetAlias());
                            sb.Append("_");
                            sb.Append(column.Value);
                            sb.Append("'");
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
                                    sb.Append("_");
                                }

                                sb.Append(column.Value);
                                sb.Append("'");
                            }
                        }
                        else if (_query.QueryColumnAliasStrategy == QueryColumnAliasStrategy.PrefixTableNameAlways)
                        {
                            sb.Append(" AS '");
                            sb.Append(QueryHelper.GetSimplifiedSchemaAndTableName(queryElement.Table.SchemaAndTableName));
                            sb.Append(column.Value);
                            sb.Append("'");
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
            sb.Append("\r\n")
                .Append(joinType)
                .Append(" JOIN ")
                .Append(QueryHelper.GetSimplifiedSchemaAndTableName(join.Table.SchemaAndTableName))
                .Append(" ")
                .Append(join.Table.GetAlias())
                .Append(" ON ");

            if (join is Join join2)
                sb.Append(AddJoinOn(join2));

            if (join is JoinOn joinOn)
                sb.Append(AddJoinOn(joinOn));

            return sb.ToString();
        }

        private string AddJoinOn(JoinOn joinOn)
        {
            return Expression.GetExpression(joinOn.OnExpression, _query.QueryElements, joinOn);
        }

        private string AddJoinOn(Join join)
        {
            var sb = new StringBuilder();

            if (join.ColumnSource == null && join.ColumnTarget == null)
            { // auto build JOIN ON
                var fk = _query.Table.Properties.OfType<ForeignKey>().First(fk => fk.ForeignKeyColumns[0].ReferredColumn.Table.SchemaAndTableName == join.Table.SchemaAndTableName);

                foreach (var fkm in fk.ForeignKeyColumns)
                {
                    sb.Append(join.Table.GetAlias());
                    sb.Append(".");
                    sb.Append(fkm.ReferredColumn.Name);
                    sb.Append(" = ");
                    sb.Append(_query.Table.GetAlias());
                    sb.Append(".");
                    sb.Append(fkm.ForeignKeyColumn.Name);
                }
            }
            else if (join.ColumnSource != null && join.ColumnTarget != null)
            {
                sb.Append(join.Table.GetAlias());
                sb.Append(".");
                sb.Append(join.ColumnSource.Value);
                sb.Append(" = ");
                sb.Append(_query.Table.GetAlias());
                sb.Append(".");
                sb.Append(join.ColumnTarget.Value);
            }
            else if (join.ColumnSource == null && join.ColumnTarget != null)
            {
                var pk = join.Table.Properties.OfType<PrimaryKey>().FirstOrDefault();
                if (pk == null)
                    throw new ArgumentException($"Target Join table has no Primary Key. Table: {join.Table.SchemaAndTableName}, source column: {join.ColumnSource}.");

                if (pk.SqlColumns.Count > 1)
                    throw new ArgumentException($"Target Join table Primary Key with multiple columns, Table: {join.Table.SchemaAndTableName}, source column: {join.ColumnSource}.");

                var cao = pk.SqlColumns[0];

                sb.Append(join.Table.GetAlias());
                sb.Append(".");
                sb.Append(cao.SqlColumn.Name);
                sb.Append(" = ");
                sb.Append(_query.Table.GetAlias());
                sb.Append(".");
                sb.Append(join.ColumnTarget.Value);
            }
            else if (join.ColumnSource != null && join.ColumnTarget == null)
            {
                var pk = _query.Table.Properties.OfType<PrimaryKey>().FirstOrDefault();
                if (pk == null)
                    throw new ArgumentException($"Target Join table has no Primary Key. Table: {join.Table.SchemaAndTableName}, source column: {join.ColumnSource}.");

                if (pk.SqlColumns.Count > 1)
                    throw new ArgumentException($"Target Join table Primary Key with multiple columns, Table: {join.Table.SchemaAndTableName}, source column: {join.ColumnSource}.");

                var cao = pk.SqlColumns[0];

                sb.Append(join.Table.GetAlias());
                sb.Append(".");
                sb.Append(join.ColumnSource.Value);
                sb.Append(" = ");
                sb.Append(_query.Table.GetAlias());
                sb.Append(".");
                sb.Append(cao.SqlColumn.Name);
            }

            return sb.ToString();
        }

        private string AddWhere()
        {
            if (!string.IsNullOrEmpty(_query.WhereExpression))
                return "\r\nWHERE " + _query.WhereExpression;

            return null;
        }
    }
}
