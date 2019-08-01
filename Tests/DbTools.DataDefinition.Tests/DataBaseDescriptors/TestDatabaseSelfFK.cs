namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseSelfFK : DatabaseDeclaration
    {
        public static LazySqlTable Company = new LazySqlTable(() =>
        {
            var company = new SqlTableDeclaration();
            company.AddInt32("Id").SetPKIdentity();
            company.AddForeignKey(Company, "Parent");
            company.AddNVarChar("Name", 100);
            return company;
        });
    }
}