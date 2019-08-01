namespace FizzCode.DbTools.DataDefinition
{
    public static class IndexHelper
    {
        public static SqlTableDeclaration AddIndex(this SqlTableDeclaration table, string[] columnNames)
        {
            var index = new Index(table, null);

            foreach (var columnName in columnNames)
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            table.Properties.Add(index);

            return table;
        }
    }
}