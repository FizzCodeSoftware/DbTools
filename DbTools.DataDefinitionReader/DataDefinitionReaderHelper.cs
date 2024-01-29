namespace FizzCode.DbTools.DataDefinitionReader
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Base;

    public static class DataDefinitionReaderHelper
    {
        private const string DefaultSchemaNameColumn = "schema_name";
        private const string DefaultTableNameColumn = "table_name";

        public static bool SchemaAndTableNameEquals(Row row, SqlTable table)
        {
            return row.GetAs<string>(DefaultTableNameColumn) == table.SchemaAndTableName.TableName
                && (string.IsNullOrEmpty(table.SchemaAndTableName.Schema) || row.GetAs<string>(DefaultSchemaNameColumn) == table.SchemaAndTableName.Schema);
        }

        public static bool SchemaAndTableNameEquals(Row row, SqlTable table, string schemaNameColumn, string tableNameColumn)
        {
            return row.GetAs<string>(tableNameColumn) == table.SchemaAndTableName.TableName
                && (string.IsNullOrEmpty(table.SchemaAndTableName.Schema) || row.GetAs<string>(schemaNameColumn) == table.SchemaAndTableName.Schema);
        }
    }
}
