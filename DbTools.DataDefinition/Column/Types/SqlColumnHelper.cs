using FizzCode.DbTools.Configuration;

namespace FizzCode.DbTools.DataDefinition
{
    public static class SqlColumnHelper
    {
        public static SqlColumn Add(SqlVersion version, SqlTable table, string name, SqlType sqlType)
        {
            SqlColumn column;
            if (table.Columns.ContainsKey(name))
            {
                column = table[name];
            }
            else
            {
                column = new SqlColumn
                {
                    Table = table,
                    Name = name
                };
                table.Columns.Add(name, column);
            }

            column.Types.Add(version, sqlType);

            return column;
        }
    }
}