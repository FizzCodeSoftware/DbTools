using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
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

    public static T AliasView<T>(this T table, string alias = null) where T : SqlView, new()
    {
        var newView = new T
        {
            DatabaseDefinition = table.DatabaseDefinition,
            SchemaAndTableName = table.SchemaAndTableName
        };

        newView.Properties.AddRange(table.Properties);

        foreach (var column in table.Columns)
        {
            var newSqlColumn = new SqlViewColumn();
            column.CopyTo(newSqlColumn);
            newSqlColumn.View = newView;
            newView.Columns.Add(newSqlColumn);
        }

        SqlTableHelper.SetAlias(newView, alias);
        // SqlTableHelper.SetupDeclaredTable(newView);

        return newView;
    }
}
