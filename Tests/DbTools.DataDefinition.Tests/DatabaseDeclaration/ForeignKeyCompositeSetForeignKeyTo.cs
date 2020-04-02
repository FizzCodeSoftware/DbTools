namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class ForeignKeyCompositeSetForeignKeyTo : TestDatabaseDeclaration
    {
        public SqlTable OrderHeader { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("OrderHeaderDescription", 100);
        });

        public SqlTable Order { get; } = AddTable(table =>
        {
            table.AddInt32("OrderHeaderId").SetForeignKeyToTable(nameof(OrderHeader)).SetPK();
            table.AddInt32("LineNumber").SetPK();
            table.AddForeignKey(nameof(Company));
            table.AddNVarChar("OrderDescription", 100);
        });

        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
        });

        public SqlTable TopOrdersPerCompany { get; } = AddTable(table =>
        {
            table.AddInt32("Top1A");
            table.AddInt32("Top1B");
            table.AddInt32("Top2A");
            table.AddInt32("Top2B");

            table.SetForeignKeyTo(nameof(Order), new []
            {
                new ColumnReference("Top1A", "OrderHeaderId"),
                new ColumnReference("Top1B", "LineNumber"),
            });

            table.SetForeignKeyTo(nameof(Order), new []
            {
                new ColumnReference("Top2A", "OrderHeaderId"),
                new ColumnReference("Top2B", "LineNumber"),
            });
        });
    }
}
