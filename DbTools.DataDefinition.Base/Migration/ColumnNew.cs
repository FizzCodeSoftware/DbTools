namespace FizzCode.DbTools.DataDefinition.Base.Migration;

public class ColumnNew : ColumnNewOrDelete
{
    public override string ToString()
    {
        return "CN: " + base.ToString();
    }
}
