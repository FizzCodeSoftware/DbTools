namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseCircular3FK : DatabaseDeclaration
    {
        public static LazySqlTable A = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(B));
            table.AddNVarChar("Name", 100);
            return table;
        });

        public static LazySqlTable B = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(C));
            table.AddNVarChar("Name", 100);
            return table;
        });

        public static LazySqlTable C = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(A));
            table.AddNVarChar("Name", 100);
            return table;
        });
    }
}