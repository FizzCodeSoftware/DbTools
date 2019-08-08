﻿namespace FizzCode.DbTools.DataDefinition
{
    public static class DefaultValueHelper
    {
        public static SqlColumnDeclaration AddDefaultValue(this SqlColumnDeclaration column, string defaultValue)
        {
            column.Properties.Add(new DefaultValue(column, defaultValue));

            return column;
        }
    }
}