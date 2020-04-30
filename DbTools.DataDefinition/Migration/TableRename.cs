namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class TableRename : TableMigration
    {
        public string NewName { get; set; }

        public override string ToString()
        {
#pragma warning disable IDE0071 // Simplify interpolation
            return $"(Rename:){NewName} from {base.ToString()}";
#pragma warning restore IDE0071 // Simplify interpolation
        }
    }
}