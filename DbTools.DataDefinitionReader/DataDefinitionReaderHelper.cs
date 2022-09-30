namespace FizzCode.DbTools.DataDefinitionReader
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Base;

    public static class DataDefinitionReaderHelper
    {
        public static bool SchemaAndTableNameEquals(Row row, SqlTable table, string schemaNameColumn = "schema_name", string tableNameColumn = "table_name")
        {
            return row.GetAs<string>(tableNameColumn) == table.SchemaAndTableName.TableName
                && (string.IsNullOrEmpty(table.SchemaAndTableName.Schema) || row.GetAs<string>(schemaNameColumn) == table.SchemaAndTableName.Schema);
        }
    }
}
