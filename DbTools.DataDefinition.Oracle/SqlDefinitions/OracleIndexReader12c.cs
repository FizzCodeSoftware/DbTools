namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public class OracleIndexReader12c : OracleDataDefinitionElementReader
    {
        private readonly List<Row> _queryResult;

        public OracleIndexReader12c(SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
            var sqlStatement = GetStatement();
            AddSchemaNamesFilter(ref sqlStatement, "ind.table_owner");
            _queryResult = Executer.ExecuteQuery(sqlStatement).Rows.ToList();
        }

        public void GetIndexes(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetIndex(table);
        }

        public void GetIndex(SqlTable table)
        {
            Index index = null;
            var rows = _queryResult
                .Where(row => DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table, "TABLE_OWNER", "TABLE_NAME")).OrderBy(row => row.GetAs<string>("INDEX_NAME")).ThenBy(row => row.GetAs<decimal>("COLUMN_POSITION"));

            foreach (var row in rows)
            {
                if (row.GetAs<decimal>("COLUMN_POSITION") == 1)
                {
                    index = new Index(table, row.GetAs<string>("INDEX_NAME"))
                    {
                        Unique = row.GetAs<string>("UNIQUENESS") == "UNIQUE"
                    };

                    table.Properties.Add(index);
                }

                var column = table.Columns[row.GetAs<string>("COLUMN_NAME")];

                index.SqlColumns.Add(new ColumnAndOrder(column, AscDesc.Asc));
            }
        }

        private static string GetStatement()
        {
            return @"
SELECT
	ind.table_owner,
	ind.table_name,
	ind_col.column_name,
	ind_col.column_position,
	ind.index_name,
	ind.index_type,
	ind.uniqueness,
	ind.table_type
FROM all_indexes ind
INNER JOIN all_ind_columns ind_col ON ind.owner = ind_col.index_owner
	AND ind.index_name = ind_col.index_name
INNER JOIN dba_users u ON u.username = ind.table_owner

LEFT JOIN all_constraints cons ON cons.owner = ind.owner AND cons.table_name = ind.table_name AND cons.index_name = ind.index_name AND (cons.constraint_type = 'P' OR cons.constraint_type = 'U')

WHERE cons.constraint_type IS NULL";
        }
    }
}
