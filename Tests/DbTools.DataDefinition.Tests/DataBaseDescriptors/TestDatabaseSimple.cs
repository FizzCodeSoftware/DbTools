namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class TestDatabaseSimple : DatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
        });
    }

    public class TestDatabaseIndex : DatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddIndex("Name");
        });
    }

    public class TestDatabaseUniqueIndex : DatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddIndex(true, "Name");
        });
    }
}