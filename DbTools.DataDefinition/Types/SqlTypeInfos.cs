namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    public class SqlTypeInfos : Dictionary<string, SqlTypeInfo>
    {
        public void Add(SqlTypeInfo sqlTypeInfo)
        {
            Add(sqlTypeInfo.DbType, sqlTypeInfo);
        }
    }
}