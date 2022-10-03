namespace FizzCode.DbTools.QueryBuilder
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Base;

    public static class QueryHelper
    {
        public static QueryColumn[] Except(this SqlTable table, params SqlColumn[] columns)
        {
            var columnNames = columns.Select(c => c.Name);
            var result = table.Columns.Where(c => !columnNames.Contains(c.Name));
            return result.Select(r => (QueryColumn)r).ToArray();
        }
    }
}
