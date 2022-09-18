namespace FizzCode.DbTools.DataDefinition.SqlGenerator
{
    public interface ISqlGeneratorDropAndCreateDatabase
    {
        SqlStatementWithParameters CreateDatabase(string databaseName);
        string DropDatabase(string databaseName);
        SqlStatementWithParameters DropDatabaseIfExists(string databaseName);
    }
}