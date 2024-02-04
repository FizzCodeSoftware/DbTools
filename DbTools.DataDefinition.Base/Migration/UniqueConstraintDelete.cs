namespace FizzCode.DbTools.DataDefinition.Base.Migration;

public class UniqueConstraintDelete : UniqueConstraintMigration
{
    public override string ToString()
    {
        return "UD: " + base.ToString();
    }
}
