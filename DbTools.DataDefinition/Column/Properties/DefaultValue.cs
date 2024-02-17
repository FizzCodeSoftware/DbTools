using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
public class DefaultValue(SqlColumn sqlColumn, string value)
    : SqlColumnProperty(sqlColumn)
{
    public string Value { get; } = value;
}