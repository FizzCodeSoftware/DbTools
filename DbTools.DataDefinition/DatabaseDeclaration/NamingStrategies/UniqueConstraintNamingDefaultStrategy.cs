namespace FizzCode.DbTools.DataDefinition
{
    using System.Linq;

    public class UniqueConstraintNamingDefaultStrategy : IUniqueConstraintNamingStrategy
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