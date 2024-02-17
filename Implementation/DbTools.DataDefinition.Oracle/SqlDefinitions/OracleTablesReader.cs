using System.Collections.Generic;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinitionReader;

namespace FizzCode.DbTools.DataDefinition.Oracle12c;
public class OracleTablesReader(SqlExecuter.SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
    : OracleDataDefinitionElementReader(executer, schemaNames)
{
    public List<SchemaAndTableName> GetSchemaAndTableNames()
    {
        var sqlStatement = @"
SELECT t.table_name tableName, t.owner schemaName
FROM dba_tables t, dba_users u 
WHERE t.owner = u.username";

        AddSchemaNamesFilter(ref sqlStatement, "t.owner");

        return Executer.ExecuteQuery(sqlStatement)
            .ConvertAll(row => new SchemaAndTableName(
                row.GetAs<string>("SCHEMANAME"),
                Throw.IfNull(row.GetAs<string>("TABLENAME"))
            ));
    }
}