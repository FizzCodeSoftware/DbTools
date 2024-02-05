using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDeclaration;
public class IndexNamingDefaultStrategy : IIndexNamingStrategy
{
    public void SetIndexName(Index index)
    {
        if (index.SqlTableOrView.SchemaAndTableNameSafe.TableName is null)
            return;

        // TODO view index name?

        var indexNameColumnsPart = string.Join("_", index.SqlColumns.ConvertAll(co => co.SqlColumn.Name));
        index.Name = $"IX_{index.SqlTableOrView.SchemaAndTableName.TableName}_{indexNameColumnsPart}";
    }
}