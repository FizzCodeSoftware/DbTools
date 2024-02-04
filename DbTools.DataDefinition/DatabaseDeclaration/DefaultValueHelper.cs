using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
public static class DefaultValueHelper
{
    public static SqlColumn AddDefaultValue(this SqlColumn column, string defaultValue)
    {
        column.Properties.Add(new DefaultValue(column, defaultValue));

        return column;
    }
}