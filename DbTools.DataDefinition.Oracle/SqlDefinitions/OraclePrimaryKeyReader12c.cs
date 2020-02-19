namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public class OraclePrimaryKeyReader12c
    {
        private readonly SqlStatementExecuter _executer;
        private List<Row> _queryResult;

        private List<Row> QueryResult => _queryResult ?? (_queryResult = _executer.ExecuteQuery(GetKeySql()).Rows.ToList());

        public OraclePrimaryKeyReader12c(SqlStatementExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        public void GetPrimaryKey(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetPrimaryKey(table);
        }

        public void GetPrimaryKey(SqlTable table)
        {
            PrimaryKey pk = null;

            var rows = QueryResult
                .Where(row => DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table, "SCHEMA_NAME", "TABLE_NAME"));

            foreach (var row in rows)
            {
                if (row.GetAs<decimal>("POSITION") == 1)
                {
                    pk = new PrimaryKey(table, row.GetAs<string>("CONSTRAINT_NAME"));

                    table.Properties.Add(pk);
                }

                var column = table.Columns[row.GetAs<string>("COLUMN_NAME")];

                var ascDesc = AscDesc.Asc;

                pk.SqlColumns.Add(new ColumnAndOrder(column, ascDesc));
            }
        }

        private static string GetKeySql()
        {
            return @"
SELECT cols.owner AS schema_name, cols.table_name AS table_name, cols.column_name, cols.position, cons.status, cons.owner
/*, cons.constraint_type*/
, cons.constraint_name
FROM all_constraints cons, all_cons_columns cols, dba_users u
WHERE cons.constraint_type = 'P'
AND cons.owner = cols.owner
AND cons.constraint_name = cols.constraint_name
AND cons.owner = u.username
AND EXISTS (SELECT 1 FROM dba_objects o
	WHERE o.owner = u.username ) AND u.default_tablespace not in
	('SYSTEM','SYSAUX') and u.ACCOUNT_STATUS = 'OPEN'
ORDER BY cols.table_name, cols.POSITION";
        }
    }
}
