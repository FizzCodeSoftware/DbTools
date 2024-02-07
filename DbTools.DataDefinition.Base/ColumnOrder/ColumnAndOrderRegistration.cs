namespace FizzCode.DbTools.DataDefinition.Base;

public class ColumnAndOrderRegistration(string columnName, AscDesc order)
    : ColumnAndOrderBase(order)
{
    public string ColumnName { get; } = columnName;

    //public override string ColumnName => throw new System.NotImplementedException();

    public override string ToString()
    {
        return $"{ColumnName} {OrderAsKeyword}";
    }
}
