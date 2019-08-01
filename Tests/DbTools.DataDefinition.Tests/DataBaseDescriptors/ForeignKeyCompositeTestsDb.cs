namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class ForeignKeyCompositeTestsDb : DatabaseDeclaration
    {
        public static LazySqlTable OrderHeader = new LazySqlTable(() =>
        {
            var orderHeader = new SqlTableDeclaration();
            orderHeader.AddInt32("Id").SetPKIdentity();
            orderHeader.AddNVarChar("OrderHeaderDescription", 100);
            return orderHeader;
        });

        public static LazySqlTable Order = new LazySqlTable(() =>
        {
            var order = new SqlTableDeclaration();
            order.AddForeignKey(OrderHeader).SetPK();
            order.AddInt32("LineNumber").SetPK();
            order.AddForeignKey(Company);
            order.AddNVarChar("OrderDescription", 100);
            return order;
        });

        public static LazySqlTable Company = new LazySqlTable(() =>
        {
            var company = new SqlTableDeclaration();
            company.AddInt32("Id").SetPKIdentity();
            company.AddNVarChar("Name", 100);
            return company;
        });

        public static LazySqlTable TopOrdersPerCompany = new LazySqlTable(() =>
        {
            var topOrdersPerCompany = new SqlTableDeclaration();
            topOrdersPerCompany.AddForeignKey(new[] {
                new ForeignKeyGroup("Top1A", Order["OrderHeaderId"]),
                new ForeignKeyGroup("Top1B", Order["LineNumber"])
            });
            topOrdersPerCompany.AddForeignKey(new[] {
                    new ForeignKeyGroup("Top2A", Order["OrderHeaderId"]),
                    new ForeignKeyGroup("Top2B", Order["LineNumber"])
                    });
            return topOrdersPerCompany;
        });
    }
}
