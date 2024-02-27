using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDeclaration;
public interface IIndexNamingStrategy : INamingStrategy
{
    void SetIndexName(Index index);
}
