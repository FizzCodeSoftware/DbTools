namespace FizzCode.DbTools.DataDefinition.Base;

public abstract class ForeignKeyRegistrationNonExsistingColumn : ForeignKeyRegistrationBase
{
    public bool IsNullable { get; set; }

    protected ForeignKeyRegistrationNonExsistingColumn(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string? name)
        : base(table, referredTableName, name)
    {
        IsNullable = isNullable;
    }
}
