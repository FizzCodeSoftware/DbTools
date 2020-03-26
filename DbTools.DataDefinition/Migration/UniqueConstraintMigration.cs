namespace FizzCode.DbTools.DataDefinition.Migration
{

    public abstract class UniqueConstraintMigration : IMigration
    {
        public UniqueConstraint UniqueConstraint { get; set; }
    }
}
