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
            Value = column.Value;
            As = alias;
            IsDbColumn = column.IsDbColumn;
        }

        public string Value { get; set; }
        public string As { get; set; }

        public bool IsDbColumn { get; set; }

        public static implicit operator QueryColumn(SqlColumn column)
        {
            var queryColumn = new QueryColumn
            {
                Value = column.Name,
                IsDbColumn = true
            };
            return queryColumn;
        }
    }
}
