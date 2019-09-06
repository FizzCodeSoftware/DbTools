namespace FizzCode.DbTools.DataDefinition
{
    public interface IForeignKeyNamingStrategy : INamingStrategy
    {
        void SetFKName(ForeignKey fk);
        string GetFkToPkColumnName(SqlColumn referredColumn, string prefix);
    }
}
