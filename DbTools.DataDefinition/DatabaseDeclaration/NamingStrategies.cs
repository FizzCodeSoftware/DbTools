namespace FizzCode.DbTools.DataDefinition
{
    public class NamingStrategies
    {
        public IPrimaryKeyNamingStrategy PrimaryKey { get; set; } = new PrimaryKeyNamingDefaultStrategy();
        public IForeignKeyNamingStrategy ForeignKey { get; set; } = new ForeignKeyNamingDefaultStrategy();
        public IIndexNamingStrategy Index { get; set; } = new IndexNamingMsSqlDefaultStrategy();
        public IUniqueConstraintNamingStrategy UniqueConstraint { get; set; } = new UniqueConstraintNamingMsSqlDefaultStrategy();
    }
}