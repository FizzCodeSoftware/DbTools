using System;
using System.Collections.Generic;
using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.QueryBuilder.Interfaces;

namespace FizzCode.DbTools.QueryBuilder;
public class Query : QueryElement, IQuery
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

    public List<JoinBase> Joins { get; } = [];
    public List<Query> Unions { get; } = [];
    public List<QueryElement> QueryElements { get; } = [];

    public List<Filter> Filters { get; } = [];

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

    /// <summary>
    /// Left join the <paramref name="table"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the join condition, from the Foreign Key of the <paramref name="table"/> to the Primary Key of the main table of the Query. For this method, the  <paramref name="table"/> has to be one and only one Foreign Key to the Primary Key.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query LeftJoin(SqlTable table, params QueryColumn[] columns)
    {
        return LeftJoin(table, null, null, columns);
    }

    /// <summary>
    /// Left join the <paramref name="table"/> with an <paramref name="alias"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the Foreign Key of the <paramref name="table"/> to the Primary Key of the main table of the Query.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query LeftJoinAlias(SqlTable table, string alias, params QueryColumn[] columns)
    {
        return LeftJoinAlias(table, alias, null, null, columns);
    }

    /// <summary>
    /// Left join the <paramref name="table"/> with an <paramref name="alias"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> and the <paramref name="columnTarget"/>.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columnTarget">The column of the other table to join. This might be a column of the main table of the Query, or a column of the table of another QueryElement.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query LeftJoinAlias(SqlTable table, string alias, QueryColumn columnSource, QueryColumn columnTarget, params QueryColumn[] columns)
    {
        return Join(new Join(table, alias, columnSource, columnTarget, JoinType.Left, columns));
    }

    /// <summary>
    /// Left join the <paramref name="table"/> with an <paramref name="alias"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> to the Primary Key of the main table of the Query..
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query LeftJoinAlias(SqlTable table, string alias, QueryColumn columnSource, params QueryColumn[] columns)
    {
        return LeftJoinAlias(table, alias, columnSource, null, columns);
    }

    /// <summary>
    /// Left join the <paramref name="table"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> and the <paramref name="columnTarget"/>.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columnTarget">The column of the other table to join. This might be a column of the main table of the Query, or a column of the table of another QueryElement.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query LeftJoin(SqlTable table, QueryColumn columnSource, QueryColumn columnTarget, params QueryColumn[] columns)
    {
        return LeftJoinAlias(table, null, columnSource, columnTarget, columns);
    }

    /// <summary>
    /// Left join the <paramref name="table"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> to the Primary Key of the main table of the Query..
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query LeftJoinTo(SqlTable table, QueryColumn columnSource, params QueryColumn[] columns)
    {
        return LeftJoin(table, columnSource, null, columns);
    }

    /// <summary>
    /// Left join the <paramref name="table"/> with an <paramref name="alias"/>, providing the join condition.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="on">The join condition.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query LeftJoinAliasOn(SqlTable table, string alias, Expression on, params QueryColumn[] columns)
    {
        return Join(new JoinOn(table, alias, on, JoinType.Left, columns));
    }

    /// <summary>
    /// Left join the <paramref name="table"/>, providing the join condition.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="on">The join condition.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query LeftJoinOn(SqlTable table, Expression on, params QueryColumn[] columns)
    {
        return LeftJoinAliasOn(table, null, on, columns);
    }

    /// <summary>
    /// Right join the <paramref name="table"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the join condition, from the Foreign Key of the <paramref name="table"/> to the Primary Key of the main table of the Query. For this method, the  <paramref name="table"/> has to be one and only one Foreign Key to the Primary Key.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query RightJoin(SqlTable table, params QueryColumn[] columns)
    {
        return RightJoin(table, null, null, columns);
    }

    /// <summary>
    /// Right join the <paramref name="table"/> with an <paramref name="alias"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the Foreign Key of the <paramref name="table"/> to the Primary Key of the main table of the Query.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query RightJoinAlias(SqlTable table, string alias, params QueryColumn[] columns)
    {
        return RightJoinAlias(table, alias, null, null, columns);
    }

    /// <summary>
    /// Right join the <paramref name="table"/> with an <paramref name="alias"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> and the <paramref name="columnTarget"/>.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columnTarget">The column of the other table to join. This might be a column of the main table of the Query, or a column of the table of another QueryElement.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query RightJoinAlias(SqlTable table, string alias, QueryColumn columnSource, QueryColumn columnTarget, params QueryColumn[] columns)
    {
        return Join(new Join(table, alias, columnSource, columnTarget, JoinType.Right, columns));
    }

    /// <summary>
    /// Right join the <paramref name="table"/> with an <paramref name="alias"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> to the Primary Key of the main table of the Query..
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query RightJoinAlias(SqlTable table, string alias, QueryColumn columnSource, params QueryColumn[] columns)
    {
        return RightJoinAlias(table, alias, columnSource, null, columns);
    }

    /// <summary>
    /// Right join the <paramref name="table"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> and the <paramref name="columnTarget"/>.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columnTarget">The column of the other table to join. This might be a column of the main table of the Query, or a column of the table of another QueryElement.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query RightJoin(SqlTable table, QueryColumn columnSource, QueryColumn columnTarget, params QueryColumn[] columns)
    {
        return RightJoinAlias(table, null, columnSource, columnTarget, columns);
    }

    /// <summary>
    /// Right join the <paramref name="table"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> to the Primary Key of the main table of the Query..
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query RightJoinTo(SqlTable table, QueryColumn columnSource, params QueryColumn[] columns)
    {
        return RightJoin(table, columnSource, null, columns);
    }

    /// <summary>
    /// Right join the <paramref name="table"/> with an <paramref name="alias"/>, providing the join condition.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="on">The join condition.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query RightJoinAliasOn(SqlTable table, string alias, Expression on, params QueryColumn[] columns)
    {
        return Join(new JoinOn(table, alias, on, JoinType.Right, columns));
    }

    /// <summary>
    /// Right join the <paramref name="table"/>, providing the join condition.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="on">The join condition.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query RightJoinOn(SqlTable table, Expression on, params QueryColumn[] columns)
    {
        return RightJoinAliasOn(table, null, on, columns);
    }

    /// <summary>
    /// Inner join the <paramref name="table"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the join condition, from the Foreign Key of the <paramref name="table"/> to the Primary Key of the main table of the Query. For this method, the  <paramref name="table"/> has to be one and only one Foreign Key to the Primary Key.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.)</param>
    /// <returns>The Query.</returns>
    public Query InnerJoin(SqlTable table, params QueryColumn[] columns)
    {
        return InnerJoin(table, null, null, columns);
    }

    /// <summary>
    /// Inner join the <paramref name="table"/> with an <paramref name="alias"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the Foreign Key of the <paramref name="table"/> to the Primary Key of the main table of the Query.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query InnerJoinAlias(SqlTable table, string alias, params QueryColumn[] columns)
    {
        return InnerJoinAlias(table, alias, null, null, columns);
    }

    /// <summary>
    /// Inner join the <paramref name="table"/> with an <paramref name="alias"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> and the <paramref name="columnTarget"/>.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columnTarget">The column of the other table to join. This might be a column of the main table of the Query, or a column of the table of another QueryElement.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query InnerJoinAlias(SqlTable table, string alias, QueryColumn columnSource, QueryColumn columnTarget, params QueryColumn[] columns)
    {
        return Join(new Join(table, alias, columnSource, columnTarget, JoinType.Inner, columns));
    }

    /// <summary>
    /// Inner join the <paramref name="table"/> with an <paramref name="alias"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> to the Primary Key of the main table of the Query..
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query InnerJoinAlias(SqlTable table, string alias, QueryColumn columnSource, params QueryColumn[] columns)
    {
        return InnerJoinAlias(table, alias, columnSource, null, columns);
    }

    /// <summary>
    /// Inner join the <paramref name="table"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> and the <paramref name="columnTarget"/>.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columnTarget">The column of the other table to join. This might be a column of the main table of the Query, or a column of the table of another QueryElement.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query InnerJoin(SqlTable table, QueryColumn columnSource, QueryColumn columnTarget, params QueryColumn[] columns)
    {
        return InnerJoinAlias(table, null, columnSource, columnTarget, columns);
    }

    /// <summary>
    /// Inner join the <paramref name="table"/>.
    /// The <see cref="QueryBuilder"/> will automatically build the equality join condition, from the <paramref name="columnSource"/> to the Primary Key of the main table of the Query..
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="columnSource">The column of the joined table to join on.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query InnerJoinTo(SqlTable table, QueryColumn columnSource, params QueryColumn[] columns)
    {
        return InnerJoin(table, columnSource, null, columns);
    }

    /// <summary>
    /// Inner join the <paramref name="table"/> with an <paramref name="alias"/>, providing the join condition.
    /// If the table is referenced in other parts of the query, use table aliasing.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="on">The join condition.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query InnerJoinAliasOn(SqlTable table, string alias, Expression on, params QueryColumn[] columns)
    {
        return Join(new JoinOn(table, alias, on, JoinType.Inner, columns));
    }

    /// <summary>
    /// Inner join the <paramref name="table"/>, providing the join condition.
    /// </summary>
    /// <param name="table">The table to join.</param>
    /// <param name="on">The join condition.</param>
    /// <param name="columns">The columns to include in the query. Provide any number of <see cref="QueryColumn"/>s, or <see cref="SqlColumn"/>s (which will be impicitly cast as a QueryColumn.</param>
    /// <returns>The Query.</returns>
    public Query InnerJoinOn(SqlTable table, Expression on, params QueryColumn[] columns)
    {
        return InnerJoinAliasOn(table, null, on, columns);
    }

    //join subquery
    public Query InnerJoinOn(Query query, Expression on, params QueryColumn[] columns)
    {
        return InnerJoinAliasOn(query, null, on, columns);
    }

    public Query InnerJoinAliasOn(Query query, string alias, Expression on, params QueryColumn[] columns)
    {
        return Join(new JoinSubQueryOn(query, alias, on, JoinType.Inner, columns));
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

    public List<QueryColumn> GroupByColumns { get; } = [];

    public Query GroupBy(params QueryColumn[] columns)
    {
        GroupByColumns.AddRange(columns);
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

    public Query FilterBetween(QueryColumn column)
    {
        var tableOrView = QueryElements.First(qe => qe.Table.GetAlias() == column.Alias).Table;

        SqlParameter parameter;

        if (tableOrView is SqlTable table)
        {
            var sqlColumn = table.Columns[column.Value];

            parameter = new SqlParameter(sqlColumn.Table.DatabaseDefinition)
            {
                Name = column.Value
            };
            sqlColumn.Types.CopyTo(parameter.Types);
        } else if (tableOrView is SqlView view)
        {
            var sqlColumn = view.Columns[column.Value];

            parameter = new SqlParameter(sqlColumn.View.DatabaseDefinition)
            {
                Name = column.Value
            };
            sqlColumn.Types.CopyTo(parameter.Types);
        } else
        {
            throw new ArgumentException("Unknown SqlTableOrView Type.");
        }

        var filter = new Filter()
        {
            Column = column,
            Table = tableOrView,
            Parameter = parameter,
            Type = FilterType.Between
        };
        Filters.Add(filter);

        return this;
    }
}
