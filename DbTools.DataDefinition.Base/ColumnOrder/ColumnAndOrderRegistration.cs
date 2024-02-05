namespace FizzCode.DbTools.DataDefinition.Base;

public class ColumnAndOrderRegistration(string columnName, AscDesc order)
    : ColumnAndOrder(order)
{
    public string ColumnName { get; set; } = columnName;

    public override string ToString()
    {
        return $"{ColumnName} {OrderAsKeyword}";
    }
}
