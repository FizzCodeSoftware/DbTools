namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class TableDelete : TableMigration
    {
        public override string ToString()
        {
            return "(Delete:) " + base.ToString();
        }
    }
}
