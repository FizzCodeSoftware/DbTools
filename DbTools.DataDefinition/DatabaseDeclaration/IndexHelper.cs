﻿namespace FizzCode.DbTools.DataDefinition
{
    public static class IndexHelper
    {
        public static SqlTableDeclaration AddIndex(this SqlTableDeclaration table, params string[] columnNames)
        {
            var index = new Index(table, null);

            var indexNaming = table.DatabaseDeclaration?.NamingStrategies.GetNamingStrategy<IIndexNamingStrategy>();

            indexNaming?.SetIndexName(index);

            if (index.Name == null)
                table.DelayedNamingTasks.Add(new DelayedNamingIndex(index));

            foreach (var columnName in columnNames)
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            table.Properties.Add(index);

            return table;
        }
    }
}