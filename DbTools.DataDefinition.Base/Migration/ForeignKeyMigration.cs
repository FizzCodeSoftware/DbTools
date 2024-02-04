namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public abstract class ForeignKeyMigration : IMigration
{
    public ForeignKey ForeignKey { get; set; }
}
