using System;
using System.Linq;
using System.Text;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.Interfaces;
using FizzCode.DbTools.QueryBuilder.Interfaces;
using Microsoft.Extensions.Primitives;

namespace FizzCode.DbTools.QueryBuilder;
public class QueryBuilderSqlGeneratorBase : QueryBuilderBase, IQueryBuilder
{
    protected int _level;
    protected ISqlGeneratorBase SqlGeneratorBase { get; }

    public QueryBuilderSqlGeneratorBase(ISqlGeneratorBase sqlGeneratorBase)
    {
        SqlGeneratorBase = sqlGeneratorBase;
    }

    protected QueryBuilderSqlGeneratorBase CreateNewQueryBuilderSqlGenerator()
    {
        return new QueryBuilderSqlGeneratorBase(SqlGeneratorBase);
    }

    public SqlEngineVersion SqlVersion
    {
        get
        {
            return SqlGeneratorBase.SqlVersion;
        }
    }

    public string Build(IQuery query)
    {
        return Build((Query)query, 0);
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
        sb.Append(SqlGeneratorBase.GetSimplifiedSchemaAndTableName(_query.Table.SchemaAndTableName));
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

            var unionQ = CreateNewQueryBuilderSqlGenerator();

            sb.Append(unionQ.Build(union));
        }

        return sb.ToString();
    }

    protected override string AddQueryElementColumns(QueryElement queryElement, bool useAlias = false)
    {
        var columns = queryElement.GetColumns();

        if (columns is null)
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

            sb.Append(SqlGeneratorBase.GuardKeywords(column.Value));

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
                                sb.Append(SqlGeneratorBase.GetSimplifiedSchemaAndTableName(queryElement.Table.SchemaAndTableName));
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
                        sb.Append(SqlGeneratorBase.GetSimplifiedSchemaAndTableName(queryElement.Table.SchemaAndTableName));
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

    protected string AddJoin(Join join)
    {
        var sb = new StringBuilder();

        var queryTableProperties = (_query.Table as SqlTable).Properties;

        if (join.ColumnSource == null && join.ColumnTarget is null)
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
            if (pk is null)
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
        else if (join.ColumnSource != null && join.ColumnTarget is null)
        {
            var pk = queryTableProperties.OfType<PrimaryKey>().FirstOrDefault();
            if (pk is null)
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

    protected string AddWhere()
    {
        if (!string.IsNullOrEmpty(_query.WhereExpression))
            return "\r\n" + StringBuilderExtensions.Spaces(_level) + "WHERE " + _query.WhereExpression;

        return null;
    }

    protected string AddGroupBy(QueryElement queryElement)
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

    protected override string AddJoin(JoinBase join)
    {
        var sb = new StringBuilder();

        var joinType = join.JoinType.ToString().ToUpperInvariant();
        sb.AppendLine()
            .Append(_level, joinType)
            .Append(" JOIN");

        if (join is JoinSubQueryOn joinSubQueryOn)
        {
            var subQb = CreateNewQueryBuilderSqlGenerator();
            sb.AppendLine()
                .Append(_level + 1, "(")
                .Append(subQb.Build(joinSubQueryOn.SubQuery, _level + 1))
                .Append(") ")
                .Append(joinSubQueryOn.Alias);
        }
        else
        {
            sb.Append(' ')
                .Append(SqlGeneratorBase.GetSimplifiedSchemaAndTableName(join.Table.SchemaAndTableName))
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
}
