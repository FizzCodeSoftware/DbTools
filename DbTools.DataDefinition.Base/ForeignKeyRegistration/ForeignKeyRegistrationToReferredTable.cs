namespace FizzCode.DbTools.DataDefinition.Base;
public class ForeignKeyRegistrationToReferredTable(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string? fkName, List<ColumnReference> map) : ForeignKeyRegistrationNonExsistingColumn(table, referredTableName, isNullable, fkName)
{
    public List<ColumnReference> Map { get; set; } = map;
}