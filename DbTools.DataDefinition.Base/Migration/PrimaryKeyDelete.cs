namespace FizzCode.DbTools.DataDefinition.Base.Migration;

public class PrimaryKeyDelete : PrimaryKeyMigration
{
    public override string ToString()
    {
        return "PD: " + base.ToString();
    }
}
