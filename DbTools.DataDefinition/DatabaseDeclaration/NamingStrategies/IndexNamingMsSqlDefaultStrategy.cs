namespace FizzCode.DbTools.DataDefinition
{
    using System.Linq;

    public class IndexNamingMsSqlDefaultStrategy : IIndexNamingStrategy
    {
        public void SetIndexName(Index index)
        {
            var indexNameColumnsPart = string.Join("_", index.SqlColumns.Select(co => co.SqlColumn.Name).ToList());
            index.Name = $"IX_{index.SqlTable.Name}_{indexNameColumnsPart}";
        }
    }
}
