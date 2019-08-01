namespace FizzCode.DbTools.DataDefinition
{
    public interface IPrimaryKeyNamingStrategy : INamingStrategy
    {
        void SetPrimaryKeyName(PrimaryKey pk);
    }
}
