namespace FizzCode.DbTools.DataDefinition.Base;

public class ForeignKeyRegistrationToTableWithUniqueKeySingleColumn(SqlTable table, SchemaAndTableName referredTableName, string singleFkColumnName, bool isNullable, string? fkName)
    : ForeignKeyRegistrationNonExsistingColumn(table, referredTableName, isNullable, fkName)
{
    public string SingleFkColumnName { get; set; } = singleFkColumnName;
}