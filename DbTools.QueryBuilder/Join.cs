namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;

    public class Join : JoinBase
    {
        public Join(SqlTable table, QueryColumn columnTo, QueryColumn columnFrom, string alias, JoinType joinType, params QueryColumn[] columns)
            : base(table, alias, joinType, columns)
        {
            ColumnTo = columnTo;
            ColumnFrom = columnFrom;
        }

        public QueryColumn ColumnTo { get; set; }
        public QueryColumn ColumnFrom { get; set; }
    }
}
