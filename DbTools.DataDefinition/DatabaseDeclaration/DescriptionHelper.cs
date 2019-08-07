namespace FizzCode.DbTools.DataDefinition
{
    public static class DescriptionHelper
    {
        public static SqlTableDeclaration AddDescription(this SqlTableDeclaration table, string description)
        {
            var sqlTableDescription = new SqlTableDescription(table, description);
            table.Properties.Add(sqlTableDescription);

            return table;
        }

        public static SqlColumnDeclaration AddDescription(this SqlColumnDeclaration column, string description)
        {
            var sqlColumnDescription = new SqlColumnDescription(column, description);
            column.Properties.Add(sqlColumnDescription);

            return column;
        }
    }
}