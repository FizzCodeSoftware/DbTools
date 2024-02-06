namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public abstract class ColumnNewOrDelete : ColumnMigration
{
    public string? Name => SqlColumn.Name;
    public SqlType? Type => SqlColumn.Type as SqlType;

    public SqlTable Table => SqlColumn.Table;
}
