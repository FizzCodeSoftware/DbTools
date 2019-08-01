namespace FizzCode.DbTools.DataDefinition
{
    public interface IForeignKeyNamingStrategy : INamingStrategy
    {
        void SetFKName(ForeignKey fk);

        void SetFKColumnsNames(ForeignKey fk, string prefix);
    }
}
