namespace FizzCode.DbTools.DataDefinition.Base.Migration
{
    using FizzCode.DbTools.DataDefinition.Base;

    public abstract class PrimaryKeyMigration : IMigration
    {
        public PrimaryKey PrimaryKey { get; set; }

        public override string ToString()
        {
            return PrimaryKey.ToString();
        }
    }
}
