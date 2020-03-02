namespace FizzCode.DbTools.DataDefinition.SqlGenerator
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public interface ISqlGenerator
    {
        Context Context { get; }

        SqlEngineVersion Version { get; }

        SqlStatementWithParameters CreateSchema(string schemaName);

        string CreateTable(SqlTable table);

        string CreateForeignKeys(SqlTable table);

        string CreateIndexes(SqlTable table);

        string CreateUniqueConstrainsts(SqlTable table);

        SqlStatementWithParameters CreateDbTableDescription(SqlTable table);
        SqlStatementWithParameters CreateDbColumnDescription(SqlColumn column);

        string DropTable(SqlTable table);

        string DropAllViews();
        string DropAllForeignKeys();
        string DropAllTables();
        SqlStatementWithParameters DropSchemas(List<string> schemaNames, bool hard = false);

        SqlStatementWithParameters TableExists(SqlTable table);
        string TableNotEmpty(SqlTable table);

        string GenerateCreateColumn(SqlColumn column);
        string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName);
        string GuardKeywords(string name);
    }
}