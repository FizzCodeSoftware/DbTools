namespace FizzCode.DbTools.DataDefinition.Base;

public class ForeignKeyRegistrationToTableWithUniqueKey(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string? namePrefix, string? fkName)
    : ForeignKeyRegistrationNonExsistingColumn(table, referredTableName, isNullable, fkName)
{
    public string? NamePrefix { get; set; } = namePrefix;
}
