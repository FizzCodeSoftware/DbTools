namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    public static class MsSql2016
    {
        private static readonly Configuration.MsSql2016 Version = new Configuration.MsSql2016();

        private static readonly MsSqlTypeMapper2016 TypeMapper = new MsSqlTypeMapper2016();

        public static SqlTypeInfo GetSqlTypeInfo(string name)
        {
            return TypeMapper.SqlTypeInfos[name];
        }

        private static SqlColumn Add(SqlTable table, string name, SqlType sqlType)
        {
            return SqlColumnHelper.Add(Version, table, name, sqlType);
        }

        public static SqlColumn AddChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("CHAR"),
                Length = length,
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddNChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("NCHAR"),
                Length = length,
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        // TODO max length

        public static SqlColumn AddVarChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("VARCHAR"),
                Length = length,
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        // TODO max length

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

        public static SqlColumn AddBit(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("BIT"),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddTinyInt(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("TINYINT"),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddSmallInt(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("SMALLINT"),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddInt(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("INT"),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddBigInt(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("BIGINT"),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDecimal(this SqlTable table, string name, int length, int scale, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("DECIMAL"),
                IsNullable = isNullable,
                Length = length,
                Scale = scale
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddNumeric(this SqlTable table, string name, int length, int scale, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("NUMERIC"),
                IsNullable = isNullable,
                Length = length,
                Scale = scale
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddMoney(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("MONEY"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddSmallMoney(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("SMALLMONEY"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddFloat(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("FLOAT"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddReal(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("REAL"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDate(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("DATE"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("TIME"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDateTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("DATETIME"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDateTime2(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("DATETIME2"),
                IsNullable = isNullable,
                Length = length
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDateTimeOffset(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("DATETIMEOFFSET"),
                IsNullable = isNullable,
                Length = length
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddSmallDateTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("SMALLDATETIME"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddBinary(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("BINARY"),
                IsNullable = isNullable,
                Length = length
            };

            return Add(table, name, sqlType);
        }

        // TODO max legth

        public static SqlColumn AddVarBinary(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("VARBINARY"),
                IsNullable = isNullable,
                Length = length
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddImage(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("IMAGE"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddText(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("TEXT"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddNText(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("TEXT"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddUniqueIdentifier(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("UNIQUEIDENTIFIER"),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

    }
}