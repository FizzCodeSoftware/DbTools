namespace FizzCode.DbTools.DataDefinition.Base;

public abstract class SqlColumnProperty
{
    public SqlColumnBase SqlColumn { get; }

    protected SqlColumnProperty(SqlColumnBase sqlColumn)
    {
        SqlColumn = sqlColumn;
    }
}