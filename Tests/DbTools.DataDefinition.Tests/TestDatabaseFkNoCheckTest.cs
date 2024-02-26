using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.MsSql2016;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;

public class TestDatabaseFkNoCheckTest : TestDatabaseDeclaration
{
    public SqlTable Primary { get; } = AddTable(table =>
      {
          table.AddInt("Id").SetPK();
      });

    public SqlTable Foreign { get; } = AddTable(table =>
      {
          table.AddInt("Id").SetPK().SetIdentity();
          table.AddInt("PrimaryId").SetForeignKeyToTable(nameof(Primary), new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true"));
      });
}
