using FizzCode.DbTools.DataDefinition.Base;
using Index = FizzCode.DbTools.DataDefinition.Base.Index;

namespace FizzCode.DbTools.DataDeclaration;
public interface IIndexNamingStrategy : INamingStrategy
{
    void SetIndexName(Index index);
}

public interface IUniqueConstraintNamingStrategy : INamingStrategy
{
    void SetUniqueConstraintName(UniqueConstraint uniqueConstraint);
}
