namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    public static class SqLite3
    {
        public static SqlColumn AddInteger(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteInfo.Current["INTEGER"],
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddReal(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteInfo.Current["REAL"],
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddText(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteInfo.Current["TEXT"],
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddBlob(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteInfo.Current["BLOB"],
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }
    }
}