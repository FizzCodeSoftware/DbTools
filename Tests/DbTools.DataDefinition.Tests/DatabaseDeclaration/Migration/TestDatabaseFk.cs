namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

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
}
