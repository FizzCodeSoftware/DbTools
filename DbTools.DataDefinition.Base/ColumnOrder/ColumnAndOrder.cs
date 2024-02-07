namespace FizzCode.DbTools.DataDefinition.Base;

public class ColumnAndOrder(SqlColumnBase sqlColumn, AscDesc order) : ColumnAndOrderBase(order)
{
    public SqlColumnBase SqlColumn { get; } = sqlColumn;

    public override string ToString()
    {
        return $"{SqlColumn?.Name} {OrderAsKeyword}";
    }
}
