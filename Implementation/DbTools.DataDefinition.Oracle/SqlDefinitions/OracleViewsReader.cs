using System.Collections.Generic;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinitionReader;

namespace FizzCode.DbTools.DataDefinition.Oracle12c;
public class OracleViewsReader(SqlExecuter.SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
    : OracleDataDefinitionElementReader(executer, schemaNames)
{
    public List<SchemaAndTableName> GetSchemaAndTableNames()
    {
        var sqlStatement = @"
SELECT view_name as tableName, owner as schemaName
FROM sys.all_views t
WHERE 1 = 1";

        AddSchemaNamesFilter(ref sqlStatement, "t.owner");

        return Executer.ExecuteQuery(sqlStatement)
            .ConvertAll(row => new SchemaAndTableName(
                row.GetAs<string>("SCHEMANAME"),
                Throw.IfNull(row.GetAs<string>("TABLENAME"))
            ));
    }
}