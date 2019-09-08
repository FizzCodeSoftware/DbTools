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

            topOrdersPerCompany.AddForeignKey(nameof(Order), new List<ForeignKeyGroup>()
            {
                new ForeignKeyGroup("Top1A", "OrderHeaderId"),
                new ForeignKeyGroup("Top1B", "LineNumber"),
            });

            topOrdersPerCompany.AddForeignKey(nameof(Order), new List<ForeignKeyGroup>()
            {
                new ForeignKeyGroup("Top2A", "OrderHeaderId"),
                new ForeignKeyGroup("Top2B", "LineNumber"),
            });

            return topOrdersPerCompany;
        });
    }

    public class ForeignKeyCompositeSetForeignKeyVerboseTestsDb : DatabaseDeclaration
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
            topOrdersPerCompany.AddInt32("Top1A");
            topOrdersPerCompany.AddInt32("Top1B");
            topOrdersPerCompany.AddInt32("Top2A");
            topOrdersPerCompany.AddInt32("Top2B");

            var fk1 = new ForeignKey(topOrdersPerCompany, nameof(Order), "FK_TopOrdersPerCompany__Top1A__Top1B");
            fk1.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk1, topOrdersPerCompany.Columns["Top1A"], "OrderHeaderId"));
            fk1.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk1, topOrdersPerCompany.Columns["Top1B"], "LineNumber"));
            topOrdersPerCompany.Properties.Add(fk1);

            var fk2 = new ForeignKey(topOrdersPerCompany, nameof(Order), "FK_TopOrdersPerCompany__Top2A__Top2B");
            fk2.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk1, topOrdersPerCompany.Columns["Top2A"], "OrderHeaderId"));
            fk2.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk1, topOrdersPerCompany.Columns["Top2B"], "LineNumber"));
            topOrdersPerCompany.Properties.Add(fk2);

            return topOrdersPerCompany;
        });
    }

    public class ForeignKeyCompositeSetForeignKeyToTestDb : DatabaseDeclaration
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
            topOrdersPerCompany.AddInt32("Top1A");
            topOrdersPerCompany.AddInt32("Top1B");
            topOrdersPerCompany.AddInt32("Top2A");
            topOrdersPerCompany.AddInt32("Top2B");

            topOrdersPerCompany.SetForeignKeyTo(nameof(Order), new List<ForeignKeyGroup>()
            {
                new ForeignKeyGroup("Top1A", "OrderHeaderId"),
                new ForeignKeyGroup("Top1B", "LineNumber"),
            });

            return topOrdersPerCompany;
        });
    }
}
