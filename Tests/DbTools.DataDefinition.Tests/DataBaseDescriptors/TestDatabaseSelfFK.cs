namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseSelfFK : DatabaseDeclaration
    {
        public static LazySqlTable Company = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(Company), false, null, "Parent");
            table.AddNVarChar("Name", 100);
            return table;
        });
    }
}