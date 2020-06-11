namespace FizzCode.DbTools.QueryBuilder
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;

    public static class QueryHelper
    {
        // TODO temporal for now, duplicated
        public static string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName, string separator = ".")
        {
            var schema = schemaAndTableName.Schema;
            var tableName = schemaAndTableName.TableName;

            //var defaultSchema = Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema", null);
            var defaultSchema = "dbo";

            /*if (Settings.Options.ShouldUseDefaultSchema && schema == null)
                return defaultSchema + separator + tableName;

            if (!Settings.Options.ShouldUseDefaultSchema && schema == defaultSchema)
                return tableName;*/

            if (schema == defaultSchema)
                return tableName;

            if (schema != null)
                return schema + separator + tableName;

            return tableName;
        }

        public static QueryColumn[] Except(this SqlTable table, params SqlColumn[] columns)
        {
            var columnNames = columns.Select(c => c.Name);
            var result = table.Columns.Where(c => !columnNames.Contains(c.Name));
            return result.Select(r => (QueryColumn)r).ToArray();
        }
    }
}
