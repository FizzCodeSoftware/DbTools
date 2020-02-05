namespace FizzCode.DbTools.DataDefinitionExecuter
{
    public abstract class DatabaseTask
    {
        protected DatabaseTask(SqlExecuter sqlExecuter)
        {
            Executer = sqlExecuter;
        }

        protected SqlExecuter Executer { get; }
    }
}