namespace FizzCode.DbTools.DataDefinition.SqlExecuter
{
    public abstract class DatabaseTask
    {
        protected DatabaseTask(SqlStatementExecuter sqlExecuter)
        {
            Executer = sqlExecuter;
        }

        protected SqlStatementExecuter Executer { get; }
    }
}