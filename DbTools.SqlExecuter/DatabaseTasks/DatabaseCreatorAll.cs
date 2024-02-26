using FizzCode.DbTools.Interfaces;

namespace FizzCode.DbTools.SqlExecuter;

public class DatabaseCreatorAll(ISqlStatementExecuter sqlExecuter)
    : DatabaseTask(sqlExecuter)
{
    public void DropAllViews()
    {
        var sql = Executer.Generator.DropAllViews();
        Executer.ExecuteNonQuery(sql);
    }

    public void DropAllForeignKeys()
    {
        var sql = Executer.Generator.DropAllForeignKeys();
        Executer.ExecuteNonQuery(sql);
    }

    public void DropAllTables()
    {
        var sql = Executer.Generator.DropAllTables();
        Executer.ExecuteNonQuery(sql);
    }
}
