namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public abstract class PrimaryKeyMigration : IMigration
{
    public required PrimaryKey PrimaryKey { get; init; }

    public override string ToString()
    {
        return PrimaryKey.ToString();
    }
}
