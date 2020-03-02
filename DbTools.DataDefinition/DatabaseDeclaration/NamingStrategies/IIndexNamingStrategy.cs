namespace FizzCode.DbTools.DataDefinition
{
    public interface IIndexNamingStrategy : INamingStrategy
    {
        void SetIndexName(Index index);
    }

    public interface IUniqueConstraintNamingStrategy : INamingStrategy
    {
        void SetUniqueConstraintName(UniqueConstraint uniqueConstraint);
    }
}
