namespace FizzCode.DbTools.DataDefinition.Migration
{
    using FizzCode.DbTools.DataDefinition.Base;

    public abstract class ForeignKeyMigration : IMigration
    {
        public ForeignKey ForeignKey { get; set; }
    }
}
