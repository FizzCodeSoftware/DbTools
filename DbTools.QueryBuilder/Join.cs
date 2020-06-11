namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;

    public class Join : QueryElement
    {
        public Join(SqlTable table, QueryColumn columnTo, QueryColumn columnFrom, string alias, JoinType joinType, params QueryColumn[] columns)
            : base(table, alias, columns)
        {
            ColumnTo = columnTo;
            ColumnFrom = columnFrom;
            JoinType = joinType;
        }
        public QueryColumn ColumnTo { get; set; }
        public QueryColumn ColumnFrom { get; set; }
        public JoinType JoinType { get; set; }
    }
}
