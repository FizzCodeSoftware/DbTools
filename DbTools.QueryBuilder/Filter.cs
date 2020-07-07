namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;

    public class Filter
    {
        public SqlTable Table { get; set; }
        public QueryColumn Column { get; set; }
        public FilterType Type { get; set; }

        public SqlParameter Parameter { get; set; }
    }

    public class FilterExpression
    {
    }

    public enum FilterType
    {
        Equal,
        Greater,
        Lesser,
        Between
    }
}
