namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;

    public class Join : JoinBase
    {
        public Join(SqlTable table, string alias, QueryColumn columnSource, QueryColumn columnTarget, JoinType joinType, params QueryColumn[] columns)
            : base(table, alias, joinType, columns)
        {
            ColumnSource = columnSource;
            ColumnTarget = columnTarget;
        }

        public QueryColumn ColumnSource { get; set; }
        public QueryColumn ColumnTarget { get; set; }
    }
}
