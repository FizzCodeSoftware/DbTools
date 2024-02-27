using System.Linq;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.SqlExecuter;

namespace FizzCode.DbTools.DataDefinitionReader;
public class OracleTableReader12c : OracleTableOrViewReader12c
{
    public OracleTableReader12c(SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
        : base(executer, schemaNames)
    {
        var sqlStatement = GetStatement();
        AddSchemaNamesFilter(ref sqlStatement, "all_tab_columns.owner");
        QueryResult = Executer.ExecuteQuery(sqlStatement).ToLookup(x => x.GetAs<string>("SCHEMAANDTABLENAME")!);
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
            column.Name = Throw.IfNull(row.GetAs<string>("COLUMN_NAME"));
            AddDefaultValue(column, row);

            sqlTable.Columns.Add(column.Name, column);
        }

        return sqlTable;
    }

    private void AddDefaultValue(SqlColumn column, Row row)
    {
        var value = row.GetAs<string>("DATA_DEFAULT");
        if (value is not null)
        {
            column.Properties.Add(new DefaultValue(column, value));
        }
    }

    private static string GetStatement()
    {
        // identity_column also shows as data_default (for example, "DataDefinitionExecuterMigrationIntegrationTests"."ISEQ$$_112882".nextval)
        // xml magic to be able to read data_default LONG type
        return @"
SELECT CONCAT(CONCAT(owner, '.'), table_name) SchemaAndTableName,
  column_id, column_name, data_type
  /*, char_length*/, char_col_decl_length, data_precision, data_scale, nullable
 , case
           when default_length is null or identity_column = 'YES' then null
           else
               extractvalue
               ( dbms_xmlgen.getxmltype
                 ( 'select data_default from all_tab_columns where owner = ''' || all_tab_columns.owner || ''' and table_name = ''' || all_tab_columns.table_name || ''' and column_name = ''' || all_tab_columns.column_name || ''''  )
               , '//text()' )
       end as data_default
  FROM all_tab_columns, dba_users u
  WHERE all_tab_columns.owner = u.username";
    }
}