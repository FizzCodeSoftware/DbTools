namespace FizzCode.DbTools.DataDeclaration
{
    using FizzCode.DbTools.DataDefinition.Base;

    public class UniqueConstraintNamingDefaultStrategy : IUniqueConstraintNamingStrategy
    {
        public void SetUniqueConstraintName(UniqueConstraint uniqueConstraint)
        {
            if (uniqueConstraint.SqlTableOrView.SchemaAndTableName.TableName == null)
                return;

            var indexNameColumnsPart = string.Join("_", uniqueConstraint.SqlColumns.ConvertAll(co => co.SqlColumn.Name));
            uniqueConstraint.Name = $"UQ_{uniqueConstraint.SqlTableOrView.SchemaAndTableName.TableName}_{indexNameColumnsPart}";
        }
    }
}