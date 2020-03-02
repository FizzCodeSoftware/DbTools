namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionReader;

    public class OracleTablesReader : OracleDataDefinitionElementReader
    {
        public OracleTablesReader(SqlExecuter.SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
        }

        public List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            var sqlStatement = @"
SELECT t.table_name tableName, t.owner schemaName
FROM dba_tables t, dba_users u 
WHERE t.owner = u.username";

            AddSchemaNamesFilter(ref sqlStatement, "t.owner");

            return Executer.ExecuteQuery(sqlStatement).Rows
                .Select(row => new SchemaAndTableName(row.GetAs<string>("SCHEMANAME"), row.GetAs<string>("TABLENAME")))
                .ToList();
        }
    }
}
