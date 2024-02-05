namespace FizzCode.DbTools.DataDefinition.Base;

public class ColumnAndOrder
{
    public SqlColumnBase? SqlColumn { get; }
    public AscDesc Order { get; set; }
    public string OrderAsKeyword => Order.ToString().ToUpperInvariant();

    protected ColumnAndOrder(AscDesc order)
    {
        Order = order;
    }

    public ColumnAndOrder(SqlColumnBase sqlColumn, AscDesc order)
        : this(order)
    {
        SqlColumn = sqlColumn;
    }

    public override string ToString()
    {
        return $"{SqlColumn?.Name} {OrderAsKeyword}";
    }
}
