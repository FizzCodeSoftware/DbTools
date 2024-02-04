using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDeclaration;
public interface IPrimaryKeyNamingStrategy : INamingStrategy
{
    void SetPrimaryKeyName(PrimaryKey pk);
}
