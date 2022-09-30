namespace FizzCode.DbTools.Interfaces
{
    public interface ISqlExecuterDropAndCreateDatabase : ISqlStatementExecuter
    {
        void CreateDatabase();
        void DropDatabaseIfExists();
        void DropDatabase();
    }
}