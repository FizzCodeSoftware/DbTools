namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionReader;

    public class Oracle12cDataDefinitionReader : GenericDataDefinitionReader
    {
        public Oracle12cDataDefinitionReader(ConnectionStringWithProvider connectionStringWithProvider, Context context, List<string> schemaNames = null)
            : base(new Oracle12cExecuter(connectionStringWithProvider, new Oracle12cGenerator(context)), schemaNames)
        {
        }

        public override DatabaseDefinition GetDatabaseDefinition()
        {
            var dd = new DatabaseDefinition(new Oracle12cTypeMapper(), new[] { GenericVersion.Generic1.GetTypeMapper() });

            Log(LogSeverity.Debug, "Reading table definitions from database.");

            foreach (var schemaAndTableName in GetSchemaAndTableNames())
                dd.AddTable(GetTableDefinition(schemaAndTableName, false));

            Log(LogSeverity.Debug, "Reading table primary keys from database.");
            new OraclePrimaryKeyReader12c(Executer, SchemaNames).GetPrimaryKey(dd);
            Logger.Log(LogSeverity.Debug, "Reading table foreign keys from database.", "Reader");
            new OracleForeignKeyReader12c(Executer, SchemaNames).GetForeignKeys(dd);

            return dd;
        }

        public override List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            var sqlStatement = @"
SELECT t.table_name tableName, t.owner schemaName
FROM dba_tables t, dba_users u 
WHERE t.owner = u.username
AND EXISTS (SELECT 1 FROM dba_objects o
WHERE o.owner = u.username ) AND default_tablespace not in
('SYSTEM','SYSAUX') and ACCOUNT_STATUS = 'OPEN'";

            AddSchemaNamesFilter(ref sqlStatement, "t.owner");

            return Executer.ExecuteQuery(sqlStatement).Rows
                .Select(row => new SchemaAndTableName(row.GetAs<string>("SCHEMANAME"), row.GetAs<string>("TABLENAME")))
                .ToList();
        }

        private OracleTableReader12c _tableReader;

        private OracleTableReader12c TableReader => _tableReader ?? (_tableReader = new OracleTableReader12c(Executer, SchemaNames));

        public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition)
        {
            var sqlTable = TableReader.GetTableDefinition(schemaAndTableName);

            if (fullDefinition)
            {
                new OraclePrimaryKeyReader12c(Executer).
                GetPrimaryKey(sqlTable);
                new OracleForeignKeyReader12c(Executer).GetForeignKeys(sqlTable);
                // TODO
                //AddTableDocumentation(sqlTable);
            }
            // TODO 
            // ColumnDocumentationReader.GetColumnDocumentation(sqlTable);

            sqlTable.SchemaAndTableName = GetSchemaAndTableNameAsToStore(sqlTable.SchemaAndTableName, Executer.Generator.Context);

            return sqlTable;
        }
    }
}
