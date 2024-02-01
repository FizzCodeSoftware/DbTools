namespace FizzCode.DbTools.DataDefinition.Base
{
    using System.Collections.Generic;
    using FizzCode.DbTools;

    public interface IDatabaseDefinition
    {
        SqlEngineVersion MainVersion { get; }
        List<SqlEngineVersion> SecondaryVersions { get; }
        List<StoredProcedure> StoredProcedures { get; }
        TypeMappers TypeMappers { get; }

        void AddTable(SqlTable sqlTable);
        void AddView(SqlView sqlTable);
        bool Contains(SchemaAndTableName schemaAndTableName);
        bool Contains(string schema, string tableName);
        IEnumerable<string> GetSchemaNames();
        SqlTable GetTable(SchemaAndTableName schemaAndTableName);
        SqlTable GetTable(string tableName);
        SqlTable GetTable(string schema, string tableName);
        List<SqlTable> GetTables();
        List<SqlView> GetViews();
    }
}