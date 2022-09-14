namespace FizzCode.DbTools.DataDefinition
{
    public static class DescriptionHelper
    {
        public static SqlTable AddDescription(this SqlTable table, string description)
        {
            var sqlTableDescription = new SqlTableDescription(table, description);
            table.Properties.Add(sqlTableDescription);
            return table;
        }

        public static SqlColumnBase AddDescription(this SqlColumnBase column, string description)
        {
            var sqlColumnDescription = new SqlColumnDescription(column, description);
            column.Properties.Add(sqlColumnDescription);

            return column;
        }
    }
}