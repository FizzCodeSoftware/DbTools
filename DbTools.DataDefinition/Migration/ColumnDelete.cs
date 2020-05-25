namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class ColumnDelete : ColumnNewOrDelete
    {
        public override string ToString()
        {
            return "CD: " + base.ToString();
        }
    }
}
