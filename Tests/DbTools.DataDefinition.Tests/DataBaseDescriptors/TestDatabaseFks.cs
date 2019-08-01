namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseFks : DatabaseDeclaration
    {
        public static LazySqlTable Child = new LazySqlTable(() =>
        {
            var company = new SqlTableDeclaration();
            company.AddInt32("Id").SetPKIdentity();
            company.AddNVarChar("Name", 100);
            company.AddForeignKey(Parent);
            return company;
        });

        public static LazySqlTable ChildChild = new LazySqlTable(() =>
        {
            var company = new SqlTableDeclaration();
            company.AddInt32("Id").SetPKIdentity();
            company.AddNVarChar("Name", 100);
            company.AddForeignKey(Child);
            return company;
        });

        public static LazySqlTable Parent = new LazySqlTable(() =>
        {
            var company = new SqlTableDeclaration();
            company.AddInt32("Id").SetPKIdentity();
            company.AddNVarChar("Name", 100);
            return company;
        });
    }
}