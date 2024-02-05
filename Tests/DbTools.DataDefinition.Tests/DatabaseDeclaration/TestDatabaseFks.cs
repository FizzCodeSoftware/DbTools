using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class TestDatabaseFks : TestDatabaseDeclaration
{
    public SqlTable Child { get; } = AddTable(table =>
      {
          table.AddInt32("Id").SetPK().SetIdentity();
          table.AddNVarChar("Name", 100);
          table.AddForeignKey(nameof(Parent));
      });

    public SqlTable ChildChild { get; } = AddTable(table =>
      {
          table.AddInt32("Id").SetPK().SetIdentity();
          table.AddNVarChar("Name", 100);
          table.AddForeignKey(nameof(Child));
      });

    public SqlTable Parent { get; } = AddTable(table =>
      {
          table.AddInt32("Id").SetPK().SetIdentity();
          table.AddNVarChar("Name", 100);
      });
}