namespace FizzCode.DbTools.SqlExecuter
{
    public interface ISqlExecuterDropAndCreateDatabase : ISqlStatementExecuter
    {
        void CreateDatabase();
        void DropDatabaseIfExists();
        void DropDatabase();
    }
}