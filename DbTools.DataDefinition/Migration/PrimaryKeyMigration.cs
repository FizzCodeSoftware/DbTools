namespace FizzCode.DbTools.DataDefinition.Migration
{
    public abstract class PrimaryKeyMigration : IMigration
    {
        public PrimaryKey PrimaryKey { get; set; }

        public override string ToString()
        {
            return PrimaryKey.ToString();
        }
    }
}
