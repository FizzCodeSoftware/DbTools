namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class ColumnNew : ColumnNewOrDelete
    {
        public override string ToString()
        {
            return "CN: " + base.ToString();
        }
    }
}
