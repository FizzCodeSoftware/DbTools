using System.Linq;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.SqlExecuter;

namespace FizzCode.DbTools.DataDefinitionReader;
public class OraclePrimaryKeyReader12c : OracleDataDefinitionElementReader
{
    private readonly RowSet _queryResult;

    public OraclePrimaryKeyReader12c(SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
        : base(executer, schemaNames)
    {
        var sqlStatement = GetKeySql();
        AddSchemaNamesFilter(ref sqlStatement, "cons.owner");
        sqlStatement += "\r\nORDER BY cols.table_name, cols.POSITION";
        _queryResult = Executer.ExecuteQuery(sqlStatement);
    }

    public void GetPrimaryKey(DatabaseDefinition dd)
    {
        foreach (var table in dd.GetTables())
            GetPrimaryKey(table);
    }

    public void GetPrimaryKey(SqlTable table)
    {
        PrimaryKey pk = null!;

        var rows = _queryResult
            .Where(row => DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table, "SCHEMA_NAME", "TABLE_NAME"))
            .ToList();

        foreach (var row in rows)
        {
            if (row.GetAs<decimal>("POSITION") == 1)
            {
                pk = new PrimaryKey(table, row.GetAs<string>("CONSTRAINT_NAME"));

                table.Properties.Add(pk);
            }

            var column = table.Columns[Throw.IfNull(row.GetAs<string>("COLUMN_NAME"))];

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
AND cons.owner = u.username";
    }
}
