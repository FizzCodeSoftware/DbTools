namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    using FizzCode.DbTools.Configuration;

    public static class SqLite3
    {
        private static SqlColumn Add(SqlType sqlType)
        {
            var sqlColumn = new SqlColumn
            {
                Table = new SqlTable() // dummy Sql Table
            };
            sqlColumn.Types.Add(SqLiteVersion.SqLite3, sqlType);

            return sqlColumn;
        }

        public static SqlColumn AddInteger(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteType3.Integer,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddReal(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteType3.Real,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddText(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteType3.Text,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddBlob(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqLiteType3.Blob,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }
    }
}