namespace FizzCode.DbTools.DataDefinition.Base.Migration;

public abstract class IndexMigration : IMigration
{
    public required Index Index { get; init; }

    public override string? ToString()
    {
        return Index.ToString();
    }
}
