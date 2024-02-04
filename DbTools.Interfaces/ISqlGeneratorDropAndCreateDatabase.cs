using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.Interfaces;
public interface ISqlGeneratorDropAndCreateDatabase
{
    SqlStatementWithParameters CreateDatabase(string databaseName);
    string DropDatabase(string databaseName);
    SqlStatementWithParameters DropDatabaseIfExists(string databaseName);
}