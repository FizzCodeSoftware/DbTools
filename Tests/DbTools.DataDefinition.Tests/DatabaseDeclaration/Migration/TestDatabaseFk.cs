using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class TestDatabaseFk : TestDatabaseDeclaration
{
    public SqlTable Primary { get; } = AddTable(table =>
    {
        table.AddInt32("Id").SetPK();
    });

    public SqlTable Foreign { get; } = AddTable(table =>
    {
        table.AddInt32("Id").SetPK().SetIdentity();
        table.AddInt32("PrimaryId").SetForeignKeyToTable(nameof(Primary), new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true"));
    });
}
