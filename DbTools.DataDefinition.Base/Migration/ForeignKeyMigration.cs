namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public abstract class ForeignKeyMigration : IMigration
{
    public required ForeignKey ForeignKey { get; init; }
}
