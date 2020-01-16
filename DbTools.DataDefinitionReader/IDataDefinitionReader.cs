namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public interface IDataDefinitionReader
    {
        public SqlVersion Version { get; }
        DatabaseDefinition GetDatabaseDefinition();
        List<SchemaAndTableName> GetSchemaAndTableNames();
        SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true);
    }
}
