namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Base;

    public static class SqlColumnHelper
    {
        public static SqlColumn Add(SqlEngineVersion version, SqlTable table, string name, SqlType sqlType)
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

            if (column.Table.DatabaseDefinition != null)
            {
                MapFromGen1(column);
            }

            return column;
        }

        public static void MapFromGen1(SqlColumnBase column)
        {
            if (column is SqlColumnFKRegistration)
                return;

            if (!column.Types.ContainsKey(GenericVersion.Generic1))
                return;

            foreach (var typeMapper in column.SqlTableOrView.DatabaseDefinition.TypeMappers.Values)
            {
                if (!column.Types.ContainsKey(typeMapper.SqlVersion))
                {
                    var othertype = typeMapper.MapFromGeneric1(column.Types[GenericVersion.Generic1]);
                    column.Types.Add(typeMapper.SqlVersion, (SqlType)othertype);
                }
            }
        }
    }
}