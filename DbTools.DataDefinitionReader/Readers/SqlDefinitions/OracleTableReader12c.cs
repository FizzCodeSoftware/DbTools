namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class OracleTableReader12c
    {
        private readonly SqlExecuter _executer;
        private ILookup<string, Row> _queryResult;
        private ILookup<string, Row> QueryResult => _queryResult ?? (_queryResult = _executer.ExecuteQuery(GetStatement()).Rows.ToLookup(x => x.GetAs<string>("SCHEMAANDTABLENAME")));

        protected OracleTypeMapper12c TypeMapper { get; } = new OracleTypeMapper12c();

        public OracleTableReader12c(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        public SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName)
        {
            var sqlTable = new SqlTable(schemaAndTableName);

            var rows = QueryResult[schemaAndTableName.SchemaAndName]
                .OrderBy(r => r.GetAs<decimal>("COLUMN_ID"));

            foreach (var row in rows)
            {
                var type = row.GetAs<string>("DATA_TYPE");
                var dataPrecisionDecimal = row.GetAs<decimal?>("DATA_PRECISION");
                var dataScaleDecimal = row.GetAs<decimal?>("DATA_SCALE");
                var dataPrecision = (int)(dataPrecisionDecimal ?? 0);
                var dataScale = (int)(dataScaleDecimal ?? 0);

                var isNullable = row.GetAs<string>("NULLABLE") != "N";

                var sqlType = TypeMapper.MapSqlTypeFromReaderInfo(type, isNullable, dataPrecision, dataScale);

                var column = new SqlColumn
                {
                    Table = sqlTable
                };
                column.Types.Add(_executer.Generator.Version, sqlType);
                column.Name = row.GetAs<string>("COLUMN_NAME");

                sqlTable.Columns.Add(column.Name, column);
            }

            return sqlTable;
        }

        private static string GetStatement()
        {
            return @"
SELECT CONCAT(CONCAT(owner, '.'), table_name) SchemaAndTableName,
  column_id, column_name, data_type
  /*, char_length*/, char_col_decl_length, data_precision, data_scale, nullable
  FROM all_tab_columns
 WHERE table_name IN (
	SELECT t.table_name
	FROM dba_tables t, dba_users u 
	WHERE t.owner = u.username
	AND EXISTS (SELECT 1 FROM dba_objects o
	WHERE o.owner = u.username ) AND default_tablespace not in
	('SYSTEM','SYSAUX') and ACCOUNT_STATUS = 'OPEN'
    )";
        }
    }
}