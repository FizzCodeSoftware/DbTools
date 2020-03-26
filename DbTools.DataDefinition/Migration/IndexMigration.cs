namespace FizzCode.DbTools.DataDefinition.Migration
{
    public abstract class IndexMigration : IMigration
    {
        public Index Index { get; set; }
    }
}
