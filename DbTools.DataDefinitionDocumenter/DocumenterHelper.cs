namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;

    public class DocumenterHelper
    {
        protected Settings Settings { get; set; }

        public DocumenterHelper(Settings settings)
        {
            Settings = settings;
        }

        public string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName, string separator = ".")
        {
            var schema = schemaAndTableName.Schema;
            var tableName = schemaAndTableName.TableName;

            var defaultSchema = Settings.SqlDialectSpecificSettings.GetAs<string>("DefaultSchema");

            if (Settings.Options.ShouldUseDefaultSchema && schema == null)
                return defaultSchema + separator + tableName;

            if (!Settings.Options.ShouldUseDefaultSchema && schema == defaultSchema)
                return tableName;

            if (schema != null)
                return schema + separator + tableName;

            return tableName;
        }
    }
}
