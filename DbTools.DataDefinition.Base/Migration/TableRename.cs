namespace FizzCode.DbTools.DataDefinition.Base.Migration;

public class TableRename : TableMigration
{
    public required string NewName { get; init; }

    public override string ToString()
    {
        return $"(Rename:){NewName} from {base.ToString()}";
    }
}