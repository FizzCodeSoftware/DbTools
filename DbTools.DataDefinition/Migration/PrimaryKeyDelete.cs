namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class PrimaryKeyDelete : PrimaryKeyMigration
    {
        public override string ToString()
        {
            return "PD: " + base.ToString();
        }
    }
}
