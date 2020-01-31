namespace FizzCode.DbTools.DataDefinition.Generic1
{
    public static class Generic1Helper
    {
        private static readonly Configuration.Generic1 Version = new Configuration.Generic1();

        private static readonly GenericTypeMapper1 TypeMapper = new GenericTypeMapper1();

        
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

        /*public static SqlColumn AddPK(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("DEFAULTPK"),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }*/

        public static SqlColumn AddFloatSmall(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new FloatSmall(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddFloatLarge(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new FloatLarge(),
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

        public static SqlColumn AddByte(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Byte(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddInt16(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Int16(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddInt32(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Int32(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddInt64(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Int64(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddNumber(this SqlTable table, string name, int? length, int? scale, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Number(),
                Length = length,
                Scale = scale,
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDate(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Date(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Time(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddDateTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new DateTime(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }
    }
}