namespace FizzCode.DbTools.DataDefinition.Base.Interfaces;
public interface IDataDefinitionReader
{
    IDatabaseDefinition GetDatabaseDefinition();
    IDatabaseDefinition GetDatabaseDefinition(IDatabaseDefinition dd);
    List<SchemaAndTableName> GetSchemaAndTableNames();
    SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true);
    List<SchemaAndTableName> GetViews();
    SqlView GetViewDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true);
}
