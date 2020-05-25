namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class UniqueConstraintNew : UniqueConstraintMigration
    {
        public override string ToString()
        {
            return "UN: " + base.ToString();
        }
    }
}
