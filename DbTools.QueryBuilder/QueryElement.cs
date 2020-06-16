namespace FizzCode.DbTools.QueryBuilder
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;

    public class QueryElement
    {
        public SqlTable Table { get; set; }
        public List<QueryColumn> QueryColumns { get; set; }

        public QueryElement(SqlTable sqlTable, params QueryColumn[] columns)
        {
            Table = sqlTable;
            QueryColumns = columns.ToList();
        }

        public QueryElement(SqlTable sqlTable, string alias, params QueryColumn[] columns)
            : this(sqlTable, columns)
        {
            if(alias == null || Table.GetAlias() != alias)
                Table = sqlTable.Alias(alias);
        }
    }
}
