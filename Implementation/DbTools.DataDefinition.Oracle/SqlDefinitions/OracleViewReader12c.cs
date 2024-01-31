namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.SqlExecuter;

    public class OracleViewReader12c : OracleTableOrViewReader12c
    {
        public OracleViewReader12c(SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
            var sqlStatement = GetStatement();
            AddSchemaNamesFilter(ref sqlStatement, "all_tab_columns.owner");
            QueryResult = Executer.ExecuteQuery(sqlStatement).ToLookup(x => x.GetAs<string>("SCHEMAANDTABLENAME"));
        }

        public SqlView GetViewDefinition(SchemaAndTableName schemaAndTableName)
        {
            var sqlView = new SqlView(schemaAndTableName);
            var rows = QueryResult[schemaAndTableName.SchemaAndName]
                .OrderBy(r => r.GetAs<decimal>("COLUMN_ID"));

            foreach (var row in rows)
            {
                var sqlType = GetSqlTypeFromRow(row);

                var column = new SqlViewColumn
                {
                    SqlTableOrView = sqlView
                };
                column.Types.Add(Executer.Generator.SqlVersion, sqlType);
                column.Name = row.GetAs<string>("COLUMN_NAME");

                sqlView.Columns.Add(column.Name, column);
            }

            return sqlView;
        }

        private static string GetStatement()
        {
            return @"
SELECT all_tab_columns.column_id, 
  CONCAT(CONCAT(all_tab_columns.owner, '.'), all_tab_columns.table_name) AS SchemaAndTableName,
  all_tab_columns.column_name, 
  all_tab_columns.data_type, 
  all_tab_columns.char_col_decl_length,
  all_tab_columns.data_length, 
  all_tab_columns.data_precision, 
  all_tab_columns.data_scale, 
  all_tab_columns.nullable
FROM sys.all_tab_columns
INNER JOIN sys.all_views v ON v.owner = all_tab_columns.owner
AND all_tab_columns.table_name = v.view_name";
        }

    }
}