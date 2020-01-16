namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    public static class SqLite3
    {
        private static readonly Configuration.SqLite3 Version = new Configuration.SqLite3();

        private static SqlColumn Add(SqlTable table, string name, SqlType sqlType)
        {
            return SqlColumnHelper.Add(Version, table, name, sqlType);
        }

        public static SqlColumn AddInteger(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteInfo.Current["INTEGER"],
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddReal(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteInfo.Current["REAL"],
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddText(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteInfo.Current["TEXT"],
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddBlob(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteInfo.Current["BLOB"],
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }
    }
}