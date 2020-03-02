namespace FizzCode.DbTools.DataDefinition.SqlExecuter
{
    public interface ISqlExecuterDropAndCreateDatabase : ISqlStatementExecuter
    {
        void CreateDatabase();
        void DropDatabaseIfExists();
        void DropDatabase();
    }
}