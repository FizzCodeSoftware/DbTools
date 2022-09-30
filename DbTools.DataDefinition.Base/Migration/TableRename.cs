namespace FizzCode.DbTools.DataDefinition.Base.Migration
{
    public class TableRename : TableMigration
    {
        public string NewName { get; set; }

        public override string ToString()
        {
            return $"(Rename:){NewName} from {base.ToString()}";
        }
    }
}