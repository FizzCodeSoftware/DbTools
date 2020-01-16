namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public interface ISqlGenerator
    {
        Context Context { get; }

        SqlVersion Version { get; }

        string CreateSchema(string schemaName);

        string CreateTable(SqlTable table);

        string CreateForeignKeys(SqlTable table);

        string CreateIndexes(SqlTable table);

        SqlStatementWithParameters CreateDbTableDescription(SqlTable table);
        SqlStatementWithParameters CreateDbColumnDescription(SqlColumn column);

        string DropTable(SqlTable table);

        string DropAllViews();
        string DropAllForeignKeys();
        string DropAllTables();
        string DropSchemas(List<string> schemaNames);

        SqlStatementWithParameters TableExists(SqlTable table);
        string TableNotEmpty(SqlTable table);
    }
}