namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.DataDefinitionReader;

    public class OracleViewsReader : OracleDataDefinitionElementReader
    {
        public OracleViewsReader(SqlExecuter.SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
        }

        public List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            var sqlStatement = @"
SELECT view_name as tableName, owner as schemaName
FROM sys.all_views t
WHERE 1 = 1";

            AddSchemaNamesFilter(ref sqlStatement, "t.owner");

            return Executer.ExecuteQuery(sqlStatement).Rows
                .ConvertAll(row => new SchemaAndTableName(row.GetAs<string>("SCHEMANAME"), row.GetAs<string>("TABLENAME")));
        }
    }
}