namespace FizzCode.DbTools.DataDefinition.Migration
{
    public abstract class ForeignKeyMigration : IMigration
    {
        public ForeignKey ForeignKey { get; set; }
    }
}
