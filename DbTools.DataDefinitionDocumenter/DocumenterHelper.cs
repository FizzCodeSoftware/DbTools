using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class DocumenterHelper(Settings settings)
{
    protected Settings Settings { get; set; } = settings;

    public string GetSimplifiedSchemaAndTableName(SqlTable? sqlTable, string? separator = ".")
    {
        Throw.InvalidOperationExceptionIfNull(sqlTable);

        return GetSimplifiedSchemaAndTableName(sqlTable.SchemaAndTableName, separator);
    }

    public string GetSimplifiedSchemaAndTableName(SchemaAndTableName? schemaAndTableName, string? separator = ".")
    {
        Throw.InvalidOperationExceptionIfNull(schemaAndTableName);

        var schema = schemaAndTableName.Schema;
        var tableName = schemaAndTableName.TableName;

        var defaultSchema = Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema", null);

        if (Settings.Options.ShouldUseDefaultSchema && schema is null)
            return defaultSchema + separator + tableName;

        if (!Settings.Options.ShouldUseDefaultSchema && schema == defaultSchema)
            return tableName;

        if (schema != null)
            return schema + separator + tableName;

        return tableName;
    }
}
