namespace FizzCode.DbTools.DataDefinition.Base;
public class DefaultValue(SqlColumn sqlColumn, string value, string? name = null)
    : SqlColumnPropertyWithName(sqlColumn, name)
{
    public string Value { get; } = value;

    public override string ToString()
    {
        return $"{SqlColumn.Name} DF: {Value}";
    }
}