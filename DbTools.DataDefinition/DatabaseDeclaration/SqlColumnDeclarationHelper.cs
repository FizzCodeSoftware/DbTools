namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    public static class SqlColumnDeclarationHelper
    {
        public static SqlColumnDeclaration AddNVarChar(this SqlTableDeclaration table, string name, int length, bool isNullable = false)
        {
            var column = new SqlColumnDeclaration { Type = SqlType.NVarchar, Name = name, Length = length, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumnDeclaration AddNChar(this SqlTableDeclaration table, string name, int length, bool isNullable = false)
        {
            var column = new SqlColumnDeclaration { Type = SqlType.NChar, Name = name, Length = length, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumnDeclaration AddBoolean(this SqlTableDeclaration table, string name, bool isNullable = false)
        {
            var column = new SqlColumnDeclaration { Type = SqlType.Boolean, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumnDeclaration AddByte(this SqlTableDeclaration table, string name, bool isNullable = false)
        {
            var column = new SqlColumnDeclaration { Type = SqlType.Byte, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumnDeclaration AddInt16(this SqlTableDeclaration table, string name, bool isNullable = false)
        {
            var column = new SqlColumnDeclaration { Type = SqlType.Int16, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumnDeclaration AddInt32(this SqlTableDeclaration table, string name, bool isNullable = false)
        {
            var column = new SqlColumnDeclaration { Type = SqlType.Int32, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumnDeclaration AddDateTime(this SqlTableDeclaration table, string name, bool isNullable = false)
        {
            var column = new SqlColumnDeclaration { Type = SqlType.DateTime, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumnDeclaration AddDate(this SqlTableDeclaration table, string name, bool isNullable = false)
        {
            var column = new SqlColumnDeclaration { Type = SqlType.Date, Name = name, IsNullable = isNullable };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static SqlColumnDeclaration AddDecimal(this SqlTableDeclaration table, string name, int scale, int precision, bool isNullable = false)
        {
            var column = new SqlColumnDeclaration { Type = SqlType.Decimal, Name = name, IsNullable = isNullable, Length = scale, Precision = precision };
            column.Table = table;
            table.Columns.Add(name, column);
            return column;
        }

        public static void SetPK(this List<SqlColumnDeclaration> sqlColumns)
        {
            var sqlColumn = sqlColumns[0];
            sqlColumn.SetPK();
        }
    }
}