namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;

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
}
