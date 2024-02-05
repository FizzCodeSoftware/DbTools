using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class SqlTableCustomPropertyDbTyped : TestDatabaseDeclaration
{
    public Table1Table Table1 { get; } = new Table1Table();
}

public class Table1Table : SqlTable
{
    public SqlColumn Id { get; } = Generic1.AddInt32().SetPK().SetIdentity();
    public SqlColumn Name { get; } = Generic1.AddNVarChar(100);
    public SqlTableCustomProperty MyCustomProperty { get; } = new MyCustomProperty();
}

public class MyCustomProperty : SqlTableCustomProperty
{
    public MyCustomProperty()
    {
    }

    public MyCustomProperty(SqlTable sqlTable)
        : base(sqlTable)
    {
    }
}
