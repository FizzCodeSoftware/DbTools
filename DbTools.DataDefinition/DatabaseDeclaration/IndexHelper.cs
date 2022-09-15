namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Base;
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

        public static SqlTable AddIndex(this SqlTable table, bool unique, params string[] columnNames)
        {
            var index = new Index(table, null, unique);

            foreach (var columnName in columnNames)
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            table.Properties.Add(index);
            return table;
        }

        public static SqlTable AddIndexWithName(this SqlTable table, bool unique, string indexName, params string[] columnNames)
        {
            var index = new Index(table, indexName, unique);

            foreach (var columnName in columnNames)
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            table.Properties.Add(index);
            return table;
        }

        public static SqlTable AddIndexWithName(this SqlTable table, bool unique, string indexName, string[] columnNames, string[] includeColumns)
        {
            var index = new Index(table, indexName, unique);

            foreach (var columnName in columnNames)
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            foreach (var columnName in includeColumns)
                index.Includes.Add(table[columnName]);

            table.Properties.Add(index);
            return table;
        }

        public static SqlTable AddIndex(this SqlTable table, string[] columnNames, string[] includeColumns, bool unique = false, string indexName = null)
        {
            var index = new Index(table, indexName, unique);

            foreach (var columnName in columnNames)
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            foreach (var columnName in includeColumns)
            {
                index.Includes.Add(table[columnName]);
            }

            table.Properties.Add(index);
            return table;
        }

        public static SqlTable AddUniqueConstraint(this SqlTable table, params string[] columnNames)
        {
            var constraint = new UniqueConstraint(table, null);

            foreach (var columnName in columnNames)
                constraint.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            table.Properties.Add(constraint);
            return table;
        }

        public static SqlTable AddUniqueConstraintWithName(this SqlTable table, string name, params string[] columnNames)
        {
            var constraint = new UniqueConstraint(table, name);

            foreach (var columnName in columnNames)
                constraint.SqlColumns.Add(new ColumnAndOrder(table.Columns[columnName], AscDesc.Asc));

            table.Properties.Add(constraint);
            return table;
        }
    }
}