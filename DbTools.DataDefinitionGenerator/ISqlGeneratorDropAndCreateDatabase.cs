namespace FizzCode.DbTools.DataDefinitionGenerator
{
    public interface ISqlGeneratorDropAndCreateDatabase : ISqlGenerator
    {
        SqlStatementWithParameters CreateDatabase(string databaseName);
        string DropDatabase(string databaseName);
        SqlStatementWithParameters DropDatabaseIfExists(string databaseName);
    }
}