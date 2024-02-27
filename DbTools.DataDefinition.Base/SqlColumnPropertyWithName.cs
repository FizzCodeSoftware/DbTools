namespace FizzCode.DbTools.DataDefinition.Base;

public abstract class SqlColumnPropertyWithName : SqlColumnProperty
{
    protected SqlColumnPropertyWithName(SqlColumnBase sqlColumn) : base(sqlColumn)
    {
    }

    protected SqlColumnPropertyWithName(SqlColumnBase sqlColumn, string? name) : base(sqlColumn)
    {
        Name = name;
    }

    public string? Name { get; set; }
}