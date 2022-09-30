namespace FizzCode.DbTools.DataDefinition.Base.Migration
{
    public class UniqueConstraintNew : UniqueConstraintMigration
    {
        public override string ToString()
        {
            return "UN: " + base.ToString();
        }
    }
}
