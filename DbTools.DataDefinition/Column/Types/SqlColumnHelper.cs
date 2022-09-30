namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
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


            var versions = new List<SqlEngineVersion>();
            if (column.SqlTableOrView.DatabaseDefinition.MainVersion != null)
                versions.Add(column.SqlTableOrView.DatabaseDefinition.MainVersion);

            if (column.SqlTableOrView.DatabaseDefinition.SecondaryVersions != null)
                versions.AddRange(column.SqlTableOrView.DatabaseDefinition.SecondaryVersions);

            foreach (var version in versions)
            {
                if (!column.Types.ContainsKey(version))
                {
                    var typeMapper = column.SqlTableOrView.DatabaseDefinition.TypeMappers[version];
                    var othertype = typeMapper.MapFromGeneric1(column.Types[GenericVersion.Generic1]);
                    column.Types.Add(typeMapper.SqlVersion, (SqlType)othertype);
                }
            }
        }
    }
}