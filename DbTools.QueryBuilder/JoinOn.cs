namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;

    public class JoinOn : JoinBase
    {
        public JoinOn(SqlTable table, Expression on, string alias, JoinType joinType, params QueryColumn[] columns)
            : base(table, alias, joinType, columns)
        {
            OnExpression = on;
        }

        public Expression OnExpression { get; set; }
    }
}
