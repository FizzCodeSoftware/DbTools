namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class TestDatabaseIndex : TestDatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddIndex("Name");
        });
    }

    public class TestDatabaseIndexMultiColumn : TestDatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name1", 100);
            table.AddNVarChar("Name2", 100);
            table.AddIndex("Name1", "Name2");
        });
    }

    public class TestDatabaseIndexMultiColumnAndInclude : TestDatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name1", 100);
            table.AddNVarChar("Name2", 100);
            table.AddNVarChar("Description1", 100);
            table.AddNVarChar("Description2", 100);
            table.AddIndex(new[] { "Name1", "Name2" }, new[] { "Description1", "Description2" });
            table.Properties.OfType<Index>().First().SqlColumns[1].Order = AscDesc.Desc;
        });
    }
}