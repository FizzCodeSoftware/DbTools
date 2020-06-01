namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class TableSorter
    {
        private class SqlTableDependency
        {
            public SqlTable SqlTable;
            public List<SqlTable> Parents;

            public override string ToString()
            {
                return SqlTable.SchemaAndTableName.SchemaAndName;
            }
        }

        internal static SortedList<int, SqlTable> GetSortedTables(List<SqlTable> tables)
        {
            var tablesWithDependencies = GetTablesWithDependencies(tables);
            var sortedTables = SortTables(tablesWithDependencies);

            return sortedTables;
        }

        private static List<SqlTableDependency> GetTablesWithDependencies(List<SqlTable> sqlTables)
        {
            var sqlTableDependencies = new List<SqlTableDependency>();

            foreach (var sqlTable in sqlTables)
            {
                var parents = sqlTable.Properties.OfType<ForeignKey>().Select(fk => fk.ReferredTable).Where(t => t != null).Distinct().ToList();

                sqlTableDependencies.Add(new SqlTableDependency() { SqlTable = sqlTable, Parents = parents });
            }

            return sqlTableDependencies;
        }

        private static SortedList<int, SqlTable> SortTables(List<SqlTableDependency> sqlTables)
        {
            var sorted = new SortedList<int, SqlTable>();
            var visited = new HashSet<SqlTable>();

            foreach (var table in sqlTables)
                Visit(table, visited, sorted);

            return sorted;
        }

        private static void Visit(SqlTableDependency current, HashSet<SqlTable> visited, SortedList<int, SqlTable> sorted)
        {
            if (!visited.Contains(current.SqlTable))
            {
                visited.Add(current.SqlTable);

                foreach (var parent in current.Parents)
                {
                    var parentWithDependencies = GetTablesWithDependencies(new List<SqlTable>() { parent }).First();
                    Visit(parentWithDependencies, visited, sorted);
                }

                sorted.Add(sorted.Count, current.SqlTable);
            }
            else if (!sorted.Values.Contains(current.SqlTable))
            {
                // circular dependency
            }
        }
    }
}
