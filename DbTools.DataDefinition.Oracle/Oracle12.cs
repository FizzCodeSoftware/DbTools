namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    public static class Oracle12c
    {
        private static SqlColumn Add(SqlType sqlType)
        {
            var sqlColumn = new SqlColumn
            {
                Table = new SqlTable() // dummy Sql Table
            };
            sqlColumn.Types.Add(OracleVersion.Oracle12c, sqlType);

            return sqlColumn;
        }

        public static SqlColumn AddChar(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.Char,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddNChar(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.NChar,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddVarChar(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.VarChar,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddVarChar2(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.VarChar2,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddNVarChar2(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.NVarChar2,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddBinaryFloat(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.BinaryFloat,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddBinaryDouble(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.BinaryDouble,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddBlob(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.Blob,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddClob(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.Clob,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddNClob(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.NClob,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddBfile(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.BFile,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddLong(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.Long,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddLongRaw(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.LongRaw,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddDate(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.Date,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddTimeStampWithTimeZone(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.TimeStampWithTimeZone,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddTimeStampWithLocalTimeZone(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = OracleType12c.TimeStampWithLocalTimeZone,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }
    }
}