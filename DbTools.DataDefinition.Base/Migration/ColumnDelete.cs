namespace FizzCode.DbTools.DataDefinition.Base.Migration
{
    public class ColumnDelete : ColumnNewOrDelete
    {
        public override string ToString()
        {
            return "CD: " + base.ToString();
        }
    }
}
