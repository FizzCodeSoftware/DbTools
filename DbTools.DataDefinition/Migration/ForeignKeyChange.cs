namespace FizzCode.DbTools.DataDefinition.Migration
{
    public partial class Comparer
    {
        public class ForeignKeyChange : ForeignKeyMigration
        {
            public ForeignKey NewForeignKey { get; set; }
        }
    }
}
