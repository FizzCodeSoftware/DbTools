namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseCircular3FK : DatabaseDeclaration
    {
        public SqlTable A {get;} = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(B));
            table.AddNVarChar("Name", 100);
        });

        public SqlTable B {get;} = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(C));
            table.AddNVarChar("Name", 100);
        });

        public SqlTable C {get;} = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(A));
            table.AddNVarChar("Name", 100);
        });
    }
}