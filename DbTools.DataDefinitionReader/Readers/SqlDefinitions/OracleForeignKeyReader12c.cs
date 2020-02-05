namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class OracleForeignKeyReader12c
    {
        private readonly SqlExecuter _executer;
        private List<Row> _queryResult;
        private List<Row> QueryResult => _queryResult ?? (_queryResult = _executer.ExecuteQuery(GetStatement()).Rows);

        public OracleForeignKeyReader12c(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        public void GetForeignKeys(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetForeignKeys(table);
        }

        private static string GetStatement()
        {
            return @"
SELECT 
		cons.owner,
		cons.constraint_name,
		cons.r_constraint_name,
		cons.table_name,
        cons.R_OWNER,
		--cons.constraint_type,
		cols.column_name,
		cols.POSITION,
		refcons.constraint_name ref_constraint_name,
		refcons.table_name ref_table_name,
		refcons.constraint_type ref_constraint_type,
		refcols.column_name ref_column_name
	FROM all_constraints cons
	JOIN all_constraints refcons ON refcons.owner = cons.r_owner
		AND refcons.constraint_name = cons.r_constraint_name
	JOIN dba_users u ON u.username = cons.owner
	
	JOIN all_cons_columns cols ON cols.owner = cons.owner
		AND cols.table_name = cons.table_name
		AND cols.constraint_name = cons.constraint_name
	JOIN all_cons_columns refcols ON refcols.owner = refcons.owner
	    AND refcols.table_name = refcons.table_name
		AND refcols.constraint_name = refcons.constraint_name
	
	WHERE
		cons.constraint_type = 'R'
		AND cols.POSITION = refcols.POSITION
	AND EXISTS (SELECT 1 FROM dba_objects o
	WHERE o.owner = u.username ) AND u.default_tablespace not in
	('SYSTEM','SYSAUX') and u.ACCOUNT_STATUS = 'OPEN'
	
	ORDER BY owner, table_name, position";
        }

        public void GetForeignKeys(SqlTable table)
        {
            var rows = QueryResult
                .Where(r => DataDefinitionReaderHelper.SchemaAndTableNameEquals(r, table, "OWNER", "TABLE_NAME"));

            foreach (var row in rows)
            {
                var fkColumn = table.Columns[row.GetAs<string>("COLUMN_NAME")];

                // TODO how to query reference in a nother schema?
                var referencedSchema = row.GetAs<string>("R_OWNER");
                var referencedTable = row.GetAs<string>("REF_TABLE_NAME");
                var referencedSchemaAndTableName = new SchemaAndTableName(referencedSchema, referencedTable);
                var referencedColumn = row.GetAs<string>("REF_COLUMN_NAME");
                var fkName = row.GetAs<string>("CONSTRAINT_NAME");

                var referencedSqlTableSchemaAndTableNameAsToStore = GenericDataDefinitionReader.GetSchemaAndTableNameAsToStore(referencedSchemaAndTableName, _executer.Generator.Context);

                var referencedSqlTable = table.DatabaseDefinition.GetTable(referencedSqlTableSchemaAndTableNameAsToStore);

                if (!table.Properties.OfType<ForeignKey>().Any(fk => fk.SqlTable.SchemaAndTableName == table.SchemaAndTableName && fk.ReferredTable.SchemaAndTableName == referencedSqlTableSchemaAndTableNameAsToStore && fk.Name == fkName))
                {
                    table.Properties.Add(new ForeignKey(table, referencedSqlTable, fkName));
                }

                var referencedSqlColumn = referencedSqlTable[referencedColumn];

                var fk = table.Properties.OfType<ForeignKey>().First(fk1 => fk1.ReferredTable.SchemaAndTableName == referencedSqlTableSchemaAndTableNameAsToStore && fk1.Name == fkName);
                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fkColumn, referencedSqlColumn));
            }
        }
    }
}
