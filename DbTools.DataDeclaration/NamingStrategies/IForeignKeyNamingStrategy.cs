namespace FizzCode.DbTools.DataDeclaration
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;

    public interface IForeignKeyNamingStrategy : INamingStrategy
    {
        void SetFKName(ForeignKey fk);
        string GetFkToPkColumnName(SqlColumn referredColumn, string prefix);
    }
}
