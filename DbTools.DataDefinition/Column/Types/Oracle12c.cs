namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    public static class Oracle12c
    {
        public static SqlTypeInfo GetSqlTypeInfo(string name)
        {
            return MsSqlInfo.Get(new Configuration.Oracle12c())[name];
        }

        public static SqlColumn AddNVarChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("NVARCHAR"),
                Length = length,
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }
    }
}