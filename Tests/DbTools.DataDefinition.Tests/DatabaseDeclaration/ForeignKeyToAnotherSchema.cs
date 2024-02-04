using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic1;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
// TODO
public class ForeignKeyToAnotherSchema : TestDatabaseDeclaration
{
    public SqlTable ChildꜗChild { get; } = AddTable(table =>
    {
        table.AddInt32("Id").SetPK().SetIdentity();
        table.AddNVarChar("Name", 100);
        table.AddForeignKey(nameof(ParentꜗParent));
    });

    public SqlTable ParentꜗParent { get; } = AddTable(table =>
    {
        table.AddInt32("Id").SetPK().SetIdentity();
        table.AddNVarChar("Name", 100);
    });
}
