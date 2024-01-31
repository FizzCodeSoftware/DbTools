namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.SqlExecuter;

    public class OracleTableReader12c : OracleTableOrViewReader12c
    {
        public OracleTableReader12c(SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
            var sqlStatement = GetStatement();
            AddSchemaNamesFilter(ref sqlStatement, "all_tab_columns.owner");
            QueryResult = Executer.ExecuteQuery(sqlStatement).ToLookup(x => x.GetAs<string>("SCHEMAANDTABLENAME"));
        }

        public SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName)
        {
            var sqlTable = new SqlTable(schemaAndTableName);

            var rows = QueryResult[schemaAndTableName.SchemaAndName]
                .OrderBy(r => r.GetAs<decimal>("COLUMN_ID"));

            foreach (var row in rows)
            {
                var sqlType = GetSqlTypeFromRow(row);

                var column = new SqlColumn
                {
                    Table = sqlTable
                };
                column.Types.Add(Executer.Generator.SqlVersion, sqlType);
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
  FROM all_tab_columns, dba_users u
 WHERE all_tab_columns.owner = u.username";
        }
    }
}