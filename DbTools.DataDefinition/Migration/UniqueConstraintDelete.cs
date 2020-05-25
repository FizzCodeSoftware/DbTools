namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class UniqueConstraintDelete : UniqueConstraintMigration
    {
        public override string ToString()
        {
            return "UD: " + base.ToString();
        }
    }
}
