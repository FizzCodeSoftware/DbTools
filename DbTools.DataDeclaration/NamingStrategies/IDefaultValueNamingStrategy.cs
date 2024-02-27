using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDeclaration;

public interface IDefaultValueNamingStrategy : INamingStrategy
{
    void SetDefaultValueName(DefaultValue defaultValue);
}