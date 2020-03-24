namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using System.Linq;

    public class UniqueConstraintNamingMsSqlDefaultStrategy : IUniqueConstraintNamingStrategy
    {
        public void SetUniqueConstraintName(UniqueConstraint uniqueConstraint)
        {
            if (uniqueConstraint.SqlTable.SchemaAndTableName.TableName == null)
                return;

            var indexNameColumnsPart = string.Join("_", uniqueConstraint.SqlColumns.Select(co => co.SqlColumn.Name).ToList());
            uniqueConstraint.Name = $"IX_{uniqueConstraint.SqlTable.SchemaAndTableName.TableName}_{indexNameColumnsPart}";
        }
    }
}