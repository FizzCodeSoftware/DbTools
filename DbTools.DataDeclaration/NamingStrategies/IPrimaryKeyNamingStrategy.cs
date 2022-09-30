namespace FizzCode.DbTools.DataDeclaration
{
    using FizzCode.DbTools.DataDefinition.Base;

    public interface IPrimaryKeyNamingStrategy : INamingStrategy
    {
        void SetPrimaryKeyName(PrimaryKey pk);
    }
}
