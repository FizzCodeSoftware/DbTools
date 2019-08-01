namespace FizzCode.DbTools.DataDefinition
{
    public interface IIndexNamingStrategy : INamingStrategy
    {
        void SetIndexName(Index index);
    }
}
