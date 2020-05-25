namespace FizzCode.DbTools.DataDefinition.Migration
{
    public abstract class UniqueConstraintMigration : IMigration
    {
        public UniqueConstraint UniqueConstraint { get; set; }

        public override string ToString()
        {
            return UniqueConstraint.ToString();
        }
    }
}
