using System;
using System.Collections.Generic;
using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public abstract class QueryElement
{
    public SqlTableOrView Table { get; set; }
    public List<QueryColumn> QueryColumns { get; set; }

    protected QueryElement(SqlTableOrView sqlTable, params QueryColumn[] columns)
    {
        Table = sqlTable;
        QueryColumns = columns.ToList();
    }

    protected QueryElement(SqlTableOrView sqlTable, string? alias, params QueryColumn[] columns)
        : this(sqlTable, columns)
    {
        if ((alias == null && Table.GetAlias() is null)
            || (alias != null && Table.GetAlias() != alias))
        {
            _ = sqlTable switch
            {
                SqlTable table => Table = table.Alias(alias),
                SqlView view => Table = view.AliasView(alias),
                _ => throw new ArgumentException("Unknown SqlTableOrView Type.")
            };

        }
    }

    public List<QueryColumn>? GetColumns()
    {
        if (QueryColumns.Count == 1 && QueryColumns[0] is None)
            return null;

        if (QueryColumns.Count == 0)
        {
            if (Table is SqlTable table)
            {
                return table.Columns.Select(c => (QueryColumn)c).ToList();
            }
            else if (Table is SqlView view)
            {
                return view.Columns.Select(c => (QueryColumn)c).ToList();
            }
            else
            {
                throw new ArgumentException("Unknown SqlTableOrView Type.");
            }
        }

        return QueryColumns;
    }
}
