namespace FizzCode.DbTools.DataDefinition
{
    public static class SqlColumnHelper
    {
        public static SqlColumn AddNVarChar(this SqlTable table, string name, int? length, bool isNullable = false)
        {
            var column = new SqlColumn { Type = SqlType.NVarchar, Name = name, Length = length, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddNChar(this SqlTable table, string name, int? length, bool isNullable = false)
        {
            var column = new SqlColumn { Type = SqlType.NChar, Name = name, Length = length, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddInt16(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn { Type = SqlType.Int16, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddInt32(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn { Table = table, Type = SqlType.Int32, Name = name, IsNullable = isNullable };
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddDateTime(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn { Type = SqlType.DateTime, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddDate(this SqlTable table, string name, bool isNullable = false)
        {
            var column = new SqlColumn { Type = SqlType.Date, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumn AddDecimal(this SqlTable table, string name, int? scale, int? precision, bool isNullable = false)
        {
            var column = new SqlColumn { Type = SqlType.Decimal, Name = name, IsNullable = isNullable, Length = scale, Precision = precision };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }
    }
}