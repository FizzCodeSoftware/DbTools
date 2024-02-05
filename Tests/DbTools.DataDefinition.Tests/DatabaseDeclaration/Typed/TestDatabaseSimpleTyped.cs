using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class TestDatabaseSimpleTyped : TestDatabaseDeclaration
{
    public Company Company { get; } = new Company();
}

public class Company : SqlTable
{
    public SqlColumn Id { get; } = Generic1.AddInt32().SetPK().SetIdentity();

    public SqlColumn Name { get; } = Generic1.AddNVarChar(100);
}