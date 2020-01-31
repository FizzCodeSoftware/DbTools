namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    public static class MsSql2016Helper
    {
        private static readonly Configuration.MsSql2016 Version = new Configuration.MsSql2016();

        private static SqlColumn Add(SqlTable table, string name, SqlType sqlType)
        {
            return SqlColumnHelper.Add(Version, table, name, sqlType);
        }

        public static SqlColumn AddChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Char(),
                Length = length,
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddNChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new NChar(),
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
                SqlTypeInfo = new VarChar(),
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
                SqlTypeInfo = new NVarChar(),
                Length = length,
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddBit(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Bit(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddTinyInt(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new TinyInt(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddSmallInt(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new SmallInt(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddInt(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Int(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddBigInt(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new BigInt(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDecimal(this SqlTable table, string name, int length, int scale, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Decimal(),
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
                SqlTypeInfo = new Numeric(),
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
                SqlTypeInfo = new Money(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddSmallMoney(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new SmallMoney(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddFloat(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Float(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddReal(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Real(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDate(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Date(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Time(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDateTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new DateTime(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDateTime2(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new DateTime2(),
                IsNullable = isNullable,
                Length = length
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDateTimeOffset(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new DateTimeOffset(),
                IsNullable = isNullable,
                Length = length
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddSmallDateTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new SmallDateTime(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddBinary(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Binary(),
                IsNullable = isNullable,
                Length = length
            };

            return Add(table, name, sqlType);
        }

        // TODO max length

        public static SqlColumn AddVarBinary(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new VarBinary(),
                IsNullable = isNullable,
                Length = length
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddImage(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Image(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddText(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Text(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddNText(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new NText(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddUniqueIdentifier(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new UniqueIdentifier(),
                IsNullable = isNullable,
            };

            return Add(table, name, sqlType);
        }
    }
}