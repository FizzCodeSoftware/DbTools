namespace FizzCode.DbTools.QueryBuilder
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;

    public class QueryElement
    {
        public SqlTable Table { get; set; }
        public string Alias { get; set; }
        public List<QueryColumn> QueryColumns { get; set; }

        public QueryElement(SqlTable table, string alias = null)
        {
            Table = table;
            SetAlias(alias);
        }

        private void SetAlias(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
            {
                Alias = alias;
                return;
            }

            var tableName = Table.SchemaAndTableName.TableName;
            var capitals = new string(tableName.Where(c => char.IsUpper(c)).ToArray());

#pragma warning disable CA1308 // Normalize strings to uppercase
            Alias = capitals.Length > 0 ? capitals.ToLowerInvariant()
                : alias ?? tableName.Substring(0, 1).ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
        }

        public QueryElement(SqlTable table, params QueryColumn[] columns)
            : this(table, null, columns)
        {
        }

        public QueryElement(SqlTable table, string alias, params QueryColumn[] columns)
            : this(table, alias)
        {
            QueryColumns = columns.ToList();
        }
    }
}
