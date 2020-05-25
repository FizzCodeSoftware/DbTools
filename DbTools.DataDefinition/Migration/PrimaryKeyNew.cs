namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class PrimaryKeyNew : PrimaryKeyMigration
    {
        public override string ToString()
        {
            return "PN: " + base.ToString();
        }
    }
}
