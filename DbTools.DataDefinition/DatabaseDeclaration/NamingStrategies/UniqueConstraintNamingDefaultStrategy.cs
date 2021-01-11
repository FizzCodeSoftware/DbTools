namespace FizzCode.DbTools.DataDefinition
{
    using System.Linq;

    public class UniqueConstraintNamingDefaultStrategy : IUniqueConstraintNamingStrategy
    {
        public void SetUniqueConstraintName(UniqueConstraint uniqueConstraint)
        {
            if (uniqueConstraint.SqlTable.SchemaAndTableName.TableName == null)
                return;

            var indexNameColumnsPart = string.Join("_", uniqueConstraint.SqlColumns.ConvertAll(co => co.SqlColumn.Name));
            uniqueConstraint.Name = $"UQ_{uniqueConstraint.SqlTable.SchemaAndTableName.TableName}_{indexNameColumnsPart}";
        }
    }
}