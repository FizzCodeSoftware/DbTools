namespace FizzCode.DbTools.DataDefinition
{
    public static class SqlColumnHelper
    {
        public static SqlColumn Add(SqlTable table, string name, SqlType sqlType)
        {
            SqlColumn column;
            if (table.Columns.ContainsKey(name))
                column = table[name];
            else
            {
                column = new SqlColumn
                {
                    Table = table,
                    Types = new SqlTypes(),
                    Name = name
                };
                table.Columns.Add(name, column);
            }

            column.Types.Add(new Common.MsSql2016(), sqlType);

            return column;
        }
    }
}