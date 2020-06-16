namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;

    public static class SqlTableExtension
    {
        public static T Alias<T>(this T table, string alias = null) where T : SqlTable, new()
        {
            var newTable = new T
            {
                DatabaseDefinition = table.DatabaseDefinition,
                SchemaAndTableName = table.SchemaAndTableName
            };

            newTable.Properties.AddRange(table.Properties);

            foreach (var column in table.Columns)
            {
                var newSqlColumn = new SqlColumn();
                column.CopyTo(newSqlColumn);
                newSqlColumn.Table = newTable;
                newTable.Columns.Add(newSqlColumn);
            }

            SqlTableHelper.SetAlias(newTable, alias);
            SqlTableHelper.SetupDeclaredTable(newTable);

            return newTable;
        }
    }
}
