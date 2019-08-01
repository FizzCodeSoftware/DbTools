namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public interface ISqlGenerator
    {
        string CreateTable(SqlTable table);

        string CreateForeignKey(SqlTable table);

        string CreateIndexes(SqlTable table);
        string DropTable(SqlTable table);

        string DropAllTables();

        string CreateDatabase(string databaseName, bool shouldSkipIfExists);
        string DropDatabase(string databaseName);
        string DropDatabaseIfExists(string databaseName);

        ISqlTypeMapper SqlTypeMapper { get; }

        string TableExists(SqlTable table);
        string TableNotEmpty(SqlTable table);
    }
}