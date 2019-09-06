namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition;

    public class ForeignKeyCompositeTestsDb : DatabaseDeclaration
    {
        public static LazySqlTable OrderHeader = new LazySqlTable(() =>
        {
            var orderHeader = new SqlTable();
            orderHeader.AddInt32("Id").SetPK().SetIdentity();
            orderHeader.AddNVarChar("OrderHeaderDescription", 100);
            return orderHeader;
        });

        public static LazySqlTable Order = new LazySqlTable(() =>
        {
            var order = new SqlTable();
            order.AddInt32("OrderHeaderId").SetForeignKeyTo(nameof(OrderHeader)).SetPK();
            order.AddInt32("LineNumber").SetPK();
            order.AddForeignKey(nameof(Company));
            order.AddNVarChar("OrderDescription", 100);
            return order;
        });

        public static LazySqlTable Company = new LazySqlTable(() =>
        {
            var company = new SqlTable();
            company.AddInt32("Id").SetPK().SetIdentity();
            company.AddNVarChar("Name", 100);
            return company;
        });

        public static LazySqlTable TopOrdersPerCompany = new LazySqlTable(() =>
        {
            var topOrdersPerCompany = new SqlTable();

            topOrdersPerCompany.AddForeignKey(nameof(Order), false, null, null, new List<ForeignKeyGroup>()
            {
                new ForeignKeyGroup("Top1A", "OrderHeaderId"),
                new ForeignKeyGroup("Top1B", "LineNumber"),
            });

            topOrdersPerCompany.AddForeignKey(nameof(Order), false, null, null, new List<ForeignKeyGroup>()
            {
                new ForeignKeyGroup("Top2A", "OrderHeaderId"),
                new ForeignKeyGroup("Top2B", "LineNumber"),
            });

            return topOrdersPerCompany;
        });
    }
}
