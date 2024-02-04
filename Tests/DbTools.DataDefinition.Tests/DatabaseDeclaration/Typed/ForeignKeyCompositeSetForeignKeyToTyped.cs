using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class ForeignKeyCompositeSetForeignKeyToTyped : TestDatabaseDeclaration
{
    public OrderHeader OrderHeader { get; } = new OrderHeader();
    public Order Order { get; } = new Order();
    public Company_ Company { get; } = new Company_();
    public TopOrdersPerCompany TopOrdersPerCompany { get; } = new TopOrdersPerCompany();

#pragma warning disable CA1034 // Nested types should not be visible
    public class Company_ : SqlTable
#pragma warning restore CA1034 // Nested types should not be visible
    {
        public SqlColumn Id { get; } = Generic1.Generic1.AddInt32().SetPK().SetIdentity();
        public SqlColumn Name { get; } = Generic1.Generic1.AddNVarChar(100);
    }
}

public class OrderHeader : SqlTable
{
    public SqlColumn Id { get; } = Generic1.Generic1.AddInt32().SetPK().SetIdentity();
    public SqlColumn OrderHeaderDescription { get; } = Generic1.Generic1.AddNVarChar(100);
}

public class Order : SqlTable
{
    public SqlColumn OrderHeaderId { get; } = Generic1.Generic1.AddInt32().SetForeignKeyToTable(nameof(OrderHeader)).SetPK();
    public SqlColumn LineNumber { get; } = Generic1.Generic1.AddInt32().SetPK();
    public SqlColumn CompanyId { get; } = Generic1.Generic1.AddInt32().SetForeignKeyToTable(nameof(Company));
    public SqlColumn OrderDescription { get; } = Generic1.Generic1.AddNVarChar(100);
}

public class TopOrdersPerCompany : SqlTable
{
    public SqlColumn Top1A { get; } = Generic1.Generic1.AddInt32();
    public SqlColumn Top1B { get; } = Generic1.Generic1.AddInt32();

    public SqlColumn Top2A { get; } = Generic1.Generic1.AddInt32();
    public SqlColumn Top2B { get; } = Generic1.Generic1.AddInt32();

#pragma warning disable IDE1006 // Naming Styles
    public ForeignKey _fk1 { get; } = Generic1.Generic1.SetForeignKeyTo(nameof(Order), new[]
#pragma warning restore IDE1006 // Naming Styles
        {
            new ColumnReference(nameof(Top1A), nameof(Order.OrderHeaderId)),
            new ColumnReference(nameof(Top1B), nameof(Order.LineNumber))
        });

#pragma warning disable IDE1006 // Naming Styles
    public ForeignKey _fk2 { get; } = Generic1.Generic1.SetForeignKeyTo(nameof(Order), new[]
#pragma warning restore IDE1006 // Naming Styles
        {
            new ColumnReference(nameof(Top2A), nameof(Order.OrderHeaderId)),
            new ColumnReference(nameof(Top2B), nameof(Order.LineNumber))
        });
}
