using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
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

        var defaultSchema = Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema", null);

        if (Settings.Options.ShouldUseDefaultSchema && schema == null)
            return defaultSchema + separator + tableName;

        if (!Settings.Options.ShouldUseDefaultSchema && schema == defaultSchema)
            return tableName;

        if (schema != null)
            return schema + separator + tableName;

        return tableName;
    }
}
