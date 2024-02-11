using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionReader;
public static class DataDefinitionReaderHelper
{
    private const string DefaultSchemaNameColumn = "schema_name";
    private const string DefaultTableNameColumn = "table_name";

    public static bool SchemaAndTableNameEquals(Row row, SqlTable table)
    {
        return row.GetAs<string>(DefaultTableNameColumn) == table.SchemaAndTableNameSafe.TableName
            && (string.IsNullOrEmpty(table.SchemaAndTableNameSafe.Schema) || row.GetAs<string>(DefaultSchemaNameColumn) == table.SchemaAndTableNameSafe.Schema);
    }

    public static bool SchemaAndTableNameEquals(Row row, SqlTable table, string schemaNameColumn, string tableNameColumn)
    {
        return row.GetAs<string>(tableNameColumn) == table.SchemaAndTableNameSafe.TableName
            && (string.IsNullOrEmpty(table.SchemaAndTableNameSafe.Schema) || row.GetAs<string>(schemaNameColumn) == table.SchemaAndTableNameSafe.Schema);
    }
}
