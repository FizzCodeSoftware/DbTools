namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    public static class Oracle12cHelper
    {
        private static readonly Configuration.Oracle12c Version = new Configuration.Oracle12c();

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

        public static SqlColumn AddVarChar2(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new VarChar2(),
                Length = length,
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }


        public static SqlColumn AddNVarChar2(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new NVarChar2(),
                Length = length,
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddBlob(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Blob(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddClob(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Clob(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddNClob(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new NClob(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddBfile(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Bfile(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddLong(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new Long(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }

        public static SqlColumn AddLongRaw(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = new LongRaw(),
                IsNullable = isNullable
            };

            return Add(table, name, sqlType);
        }
    }
}