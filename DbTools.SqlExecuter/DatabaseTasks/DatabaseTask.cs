namespace FizzCode.DbTools.SqlExecuter
{
    using FizzCode.DbTools.Interfaces;

    public abstract class DatabaseTask
    {
        protected DatabaseTask(ISqlStatementExecuter sqlExecuter)
        {
            Executer = sqlExecuter;
        }

        protected ISqlStatementExecuter Executer { get; }
    }
}