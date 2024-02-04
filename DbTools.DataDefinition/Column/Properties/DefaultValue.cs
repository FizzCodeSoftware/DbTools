using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
public class DefaultValue : SqlColumnProperty
{
    public string Value { get; }

    public DefaultValue(SqlColumn sqlColumn, string value)
        : base(sqlColumn)
    {
        Value = value;
    }
}