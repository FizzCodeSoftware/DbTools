namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Base;

    public static class DefaultValueHelper
    {
        public static SqlColumn AddDefaultValue(this SqlColumn column, string defaultValue)
        {
            column.Properties.Add(new DefaultValue(column, defaultValue));

            return column;
        }
    }
}