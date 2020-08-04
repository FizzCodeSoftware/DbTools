namespace FizzCode.DbTools.QueryBuilder
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;

    public abstract class QueryElement
    {
        public SqlTable Table { get; set; }
        public List<QueryColumn> QueryColumns { get; set; }

        protected QueryElement(SqlTable sqlTable, params QueryColumn[] columns)
        {
            Table = sqlTable;
            QueryColumns = columns.ToList();
        }

        protected QueryElement(SqlTable sqlTable, string alias, params QueryColumn[] columns)
            : this(sqlTable, columns)
        {
            if ((alias == null && Table.GetAlias() == null)
                || (alias != null && Table.GetAlias() != alias))
            {
                Table = sqlTable.Alias(alias);
            }
        }

        public List<QueryColumn> GetColumns()
        {
            if (QueryColumns.Count == 1 && QueryColumns[0] is None)
                return null;

            if (QueryColumns.Count == 0)
                return Table.Columns.Select(c => (QueryColumn)c).ToList();

            return QueryColumns;
        }
    }
}
