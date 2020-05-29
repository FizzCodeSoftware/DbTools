namespace FizzCode.DbTools.DataDefinition.Tests.TestDatabaseUniqueIndexTyped
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class TestDatabaseUniqueIndexTyped : TestDatabaseDeclaration
    {
        public Company Company { get; } = new Company();
    }

    public class Company : SqlTable
    {
        public SqlColumn Id { get; } = Generic1Columns.AddInt32().SetPK().SetIdentity();
        public SqlColumn Name { get; } = Generic1Columns.AddNVarChar(100);

        /*public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddIndex(true, "Name");
        });*/
    }
}