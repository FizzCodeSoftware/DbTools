namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;

    public class JoinOn : JoinBase
    {
        public JoinOn(SqlTableOrView table, string alias, Expression on, JoinType joinType, params QueryColumn[] columns)
            : base(table, alias, joinType, columns)
        {
            OnExpression = on;
        }

        public Expression OnExpression { get; set; }
    }
}
