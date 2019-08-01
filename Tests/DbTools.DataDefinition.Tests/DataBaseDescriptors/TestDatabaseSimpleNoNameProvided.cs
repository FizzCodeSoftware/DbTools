namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseSimpleNoNameProvided : DatabaseDeclaration
    {
        public static LazySqlTable Company = new LazySqlTable(() =>
        {
            var company = new SqlTableDeclaration();
            company.AddInt32("Id").SetPKIdentity();
            company.AddNVarChar("Name", 100);
            return company;
        });
    }
}