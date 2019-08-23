namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public interface ISqlGenerator
    {
        string CreateSchema(string schemaName);

        string CreateTable(SqlTable table);

        string CreateForeignKeys(SqlTable table);

        string CreateIndexes(SqlTable table);

        SqlStatementWithParameters CreateDbTableDescription(SqlTable table);
        SqlStatementWithParameters CreateDbColumnDescription(SqlColumn column);

        string DropTable(SqlTable table);

        string DropAllTables();

        string CreateDatabase(string databaseName, bool shouldSkipIfExists);
        string DropDatabase(string databaseName);
        string DropDatabaseIfExists(string databaseName);

        ISqlTypeMapper SqlTypeMapper { get; }

        SqlStatementWithParameters TableExists(SqlTable table);
        string TableNotEmpty(SqlTable table);
    }
}