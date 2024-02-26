namespace FizzCode.DbTools.DataDefinition.Base;

public class PrimaryKey(SqlTable sqlTable, string? name)
    : IndexBase<SqlTable>(sqlTable, name)
{
    public SqlTable SqlTable { get => SqlTableOrView!; }

    public override string ToString()
    {
        return $"{GetColumnsInString()} on {SqlTableOrView?.SchemaAndTableName}";
    }
}
