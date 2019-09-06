namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseFks : DatabaseDeclaration
    {
        public static LazySqlTable Child = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddForeignKey(nameof(Parent));
            return table;
        });

        public static LazySqlTable ChildChild = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddForeignKey(nameof(Child));
            return table;
        });

        public static LazySqlTable Parent = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            return table;
        });
    }
}