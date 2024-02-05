using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition.Base.Interfaces;
public interface IDataDefinitionReader
{
    IDatabaseDefinition GetDatabaseDefinition();
    List<SchemaAndTableName> GetSchemaAndTableNames();
    SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true);
    List<SchemaAndTableName> GetViews();
    SqlView GetViewDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true);
}
