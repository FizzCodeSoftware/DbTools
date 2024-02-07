namespace FizzCode.DbTools.DataDefinition.Base;

public abstract class ColumnAndOrderBase(AscDesc order)
{
    public AscDesc Order { get; set; } = order;

    public string OrderAsKeyword => Order.ToString().ToUpperInvariant();
}