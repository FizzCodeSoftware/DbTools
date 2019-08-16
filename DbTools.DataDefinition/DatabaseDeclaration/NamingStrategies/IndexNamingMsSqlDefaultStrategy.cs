namespace FizzCode.DbTools.DataDefinition
{
    using System.Linq;

    public class IndexNamingMsSqlDefaultStrategy : IIndexNamingStrategy
    {
        public void SetIndexName(Index index)
        {
            if (index.SqlTable.SchemaAndTableName.TableName == null)
                return;

            var indexNameColumnsPart = string.Join("_", index.SqlColumns.Select(co => co.SqlColumn.Name).ToList());
            index.Name = $"IX_{index.SqlTable.SchemaAndTableName.TableName}_{indexNameColumnsPart}";
        }
    }
}
