namespace FizzCode.DbTools.DataDefinition.Base.Migration;

public abstract class UniqueConstraintMigration : IMigration
{
    public required UniqueConstraint UniqueConstraint { get; init; }

    public override string ToString()
    {
        return UniqueConstraint.ToString();
    }
}
