namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition;

    public interface IDataDefinitionReader
    {
        DatabaseDefinition GetDatabaseDefinition();
        List<SchemaAndTableName> GetSchemaAndTableNames();
        SqlTable GetTableDefinition(string tableName, bool fullDefinition = true);
    }
}
