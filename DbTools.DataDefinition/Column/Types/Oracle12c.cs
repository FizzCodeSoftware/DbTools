namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    public static class Oracle12c
    {
        private static readonly Configuration.Oracle12c Version = new Configuration.Oracle12c();

        public static SqlTypeInfo GetSqlTypeInfo(string name)
        {
            return OracleInfo.Get(new Configuration.Oracle12c())[name];
        }

        private static SqlColumn Add(SqlTable table, string name, SqlType sqlType)
        {
            return SqlColumnHelper.Add(Version, table, name, sqlType);
        }

        public static SqlColumn AddNVarChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("NVARCHAR"),
                Length = length,
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }
    }
}