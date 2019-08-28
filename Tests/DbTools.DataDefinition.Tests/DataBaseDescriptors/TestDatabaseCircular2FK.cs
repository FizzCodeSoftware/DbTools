namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseCircular2FK : DatabaseDeclaration
    {
        public static LazySqlTable A = new LazySqlTable(() =>
        {
            var table = new SqlTableDeclaration();
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(B);
            table.AddNVarChar("Name", 100);
            return table;
        });

        public static LazySqlTable B = new LazySqlTable(() =>
        {
            var table = new SqlTableDeclaration();
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(A);
            table.AddNVarChar("Name", 100);
            return table;
        });
    }
}