namespace FizzCode.DbTools.DataDefinition
{
    public static class SqlColumnHelper
    {
        public static SqlColumn AddNVarChar(this SqlTable table, string name, int? length, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.NVarchar,
                Name = name,
                Length = length,
                IsNullable = isNullable
            };
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddNChar(this SqlTable table, string name, int? length, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.NChar,
                Name = name,
                Length = length,
                IsNullable = isNullable
            };
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddInt16(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Int16,
                Name = name,
                IsNullable = isNullable
            };
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddInt32(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Int32,
                Name = name,
                IsNullable = isNullable
            };
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddDateTime(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.DateTime,
                Name = name,
                IsNullable = isNullable
            };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddDate(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Date,
                Name = name,
                IsNullable = isNullable
            };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddDecimal(this SqlTable table, string name, int? scale, int? precision, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Decimal,
                Name = name,
                IsNullable = isNullable,
                Length = scale,
                Precision = precision
            };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddVarChar(this SqlTable table, string name, int? length, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Varchar,
                Name = name,
                Length = length,
                IsNullable = isNullable
            };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddChar(this SqlTable table, string name, int? length, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Char,
                Name = name,
                Length = length,
                IsNullable = isNullable
            };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddBoolean(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Boolean,
                Name = name,
                IsNullable = isNullable
            };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddByte(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Byte,
                Name = name,
                IsNullable = isNullable
            };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddInt64(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Int64,
                Name = name,
                IsNullable = isNullable
            };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddDateTimeOffset(this SqlTable table, string name, int precision, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.DateTimeOffset,
                Name = name,
                Precision = precision,
                IsNullable = isNullable
            };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddDouble(this SqlTable table, string name, int? precision, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Double,
                Name = name,
                Precision = precision,
                IsNullable = isNullable
            };
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddImage(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Image,
                Name = name,
                IsNullable = isNullable
            };
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddGuid(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn
            {
                Table = table,
                Type = SqlType.Guid,
                Name = name,
                IsNullable = isNullable
            };
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddXml(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn { Type = SqlType.Xml, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }
    }
}