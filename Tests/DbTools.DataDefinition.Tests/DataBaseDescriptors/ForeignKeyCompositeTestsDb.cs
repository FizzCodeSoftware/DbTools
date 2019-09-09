namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition;

    public class ForeignKeyCompositeTestsDb : DatabaseDeclaration
    {
        public SqlTable OrderHeader { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("OrderHeaderDescription", 100);
        });

        public SqlTable Order { get; } = AddTable(table =>
        {
            table.AddInt32("OrderHeaderId").SetForeignKeyTo(nameof(OrderHeader)).SetPK();
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
            table.AddForeignKey(nameof(Order), false, null, null, new List<ForeignKeyGroup>()
            {
                new ForeignKeyGroup("Top1A", "OrderHeaderId"),
                new ForeignKeyGroup("Top1B", "LineNumber"),
            });

            table.AddForeignKey(nameof(Order), false, null, null, new List<ForeignKeyGroup>()
            {
                new ForeignKeyGroup("Top2A", "OrderHeaderId"),
                new ForeignKeyGroup("Top2B", "LineNumber"),
            });
        });
    }
}
