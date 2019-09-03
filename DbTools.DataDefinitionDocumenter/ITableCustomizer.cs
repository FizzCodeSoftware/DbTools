namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.DataDefinition;

    public interface ITableCustomizer
    {
        bool ShouldSkip(SchemaAndTableName tableName);
        string Category(SchemaAndTableName tableName);
        string BackGroundColor(SchemaAndTableName tableName);
    }
}
