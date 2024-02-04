namespace FizzCode.DbTools.DataDefinition.Base.Migration;

public class PrimaryKeyNew : PrimaryKeyMigration
{
    public override string ToString()
    {
        return "PN: " + base.ToString();
    }
}
