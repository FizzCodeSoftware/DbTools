using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDeclaration;

public interface IUniqueConstraintNamingStrategy : INamingStrategy
{
    void SetUniqueConstraintName(UniqueConstraint uniqueConstraint);
}
