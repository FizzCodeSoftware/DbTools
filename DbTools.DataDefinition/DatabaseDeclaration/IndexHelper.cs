namespace FizzCode.DbTools.DataDefinition
{
    public static class IndexHelper
    {
        public static SqlTable AddIndex(this SqlTable table, params string[] columnNames)
        {
            var index = new Index(table, null);

            foreach (var columnName in columnNames)
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            table.Properties.Add(index);
            return table;
        }

        public static SqlTable AddIndex(this SqlTable table, string[] columnNames, string[] includeColumns)
        {
            var index = new Index(table, null);

            foreach (var columnName in columnNames)
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            foreach (var columnName in includeColumns)
            {
                index.Includes.Add(table[columnName]);
            }

            table.Properties.Add(index);
            return table;
        }
    }
}