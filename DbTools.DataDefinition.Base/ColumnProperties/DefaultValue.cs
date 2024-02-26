namespace FizzCode.DbTools.DataDefinition.Base;
public class DefaultValue(SqlColumn sqlColumn, string value)
    : SqlColumnProperty(sqlColumn)
{
    public string Value { get; } = value;
}