namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;

    public abstract class TypeConverter
    {
        public abstract SqlColumn AddNVarChar(SqlTable table, string name, int length, bool isNullable = false);

      
    }
}