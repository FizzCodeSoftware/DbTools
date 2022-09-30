namespace FizzCode.DbTools.Interfaces
{
    using FizzCode.DbTools.Common;

    public interface ISqlGeneratorDropAndCreateDatabase
    {
        SqlStatementWithParameters CreateDatabase(string databaseName);
        string DropDatabase(string databaseName);
        SqlStatementWithParameters DropDatabaseIfExists(string databaseName);
    }
}