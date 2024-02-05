namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public abstract class ColumnMigration : IMigration
{
    public required SqlColumn SqlColumn { get; init; }

    public override string ToString()
    {
        return SqlColumn.ToString();
    }
}
