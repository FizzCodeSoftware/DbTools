using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public interface ITableCustomizer
{
    bool ShouldSkip(SchemaAndTableName tableName);
    string? Category(SchemaAndTableName tableName);
    string? BackGroundColor(SchemaAndTableName tableName);
}