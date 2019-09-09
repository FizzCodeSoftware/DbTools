namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseFks : DatabaseDeclaration
    {
        public SqlTable Child {get;} = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddForeignKey(nameof(Parent));
        });

        public SqlTable ChildChild {get;} = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddForeignKey(nameof(Child));
        });

        public SqlTable Parent {get;} = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
        });
    }
}