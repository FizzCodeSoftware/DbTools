using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDeclaration;
public interface IForeignKeyNamingStrategy : INamingStrategy
{
    void SetFKName(ForeignKey fk);
    string GetFkToPkColumnName(SqlColumn referredColumn, string? prefix);
}
