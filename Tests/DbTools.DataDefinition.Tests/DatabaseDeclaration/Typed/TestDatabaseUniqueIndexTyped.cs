using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests.TestDatabaseUniqueIndexTyped;
public class TestDatabaseUniqueIndexTyped : TestDatabaseDeclaration
{
    public Company Company { get; } = new Company();
}

public class Company : SqlTable
{
    public SqlColumn Id { get; } = Generic1.Generic1.AddInt32().SetPK().SetIdentity();
    public SqlColumn Name { get; } = Generic1.Generic1.AddNVarChar(100);

#pragma warning disable IDE1006 // Naming Styles
    public Index _i { get; } = Generic1.Generic1.AddIndex(true, nameof(Name));
#pragma warning restore IDE1006 // Naming Styles
}