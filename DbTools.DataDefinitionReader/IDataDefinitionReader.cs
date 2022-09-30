namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition.Base;

    public interface IDataDefinitionReader
    {
        IDatabaseDefinition GetDatabaseDefinition();
        List<SchemaAndTableName> GetSchemaAndTableNames();
        SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true);
        List<SchemaAndTableName> GetViews();
        SqlView GetViewDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true);
    }
}
