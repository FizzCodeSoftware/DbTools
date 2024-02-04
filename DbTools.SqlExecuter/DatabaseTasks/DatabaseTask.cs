using FizzCode.DbTools.Interfaces;

namespace FizzCode.DbTools.SqlExecuter;
public abstract class DatabaseTask
{
    protected DatabaseTask(ISqlStatementExecuter sqlExecuter)
    {
        Executer = sqlExecuter;
    }

    protected ISqlStatementExecuter Executer { get; }
}