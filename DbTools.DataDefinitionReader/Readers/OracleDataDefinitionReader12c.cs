namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public class OracleDataDefinitionReader12c : GenericDataDefinitionReader
    {
        public OracleDataDefinitionReader12c(ConnectionStringWithProvider connectionStringWithProvider, Context context) : base(connectionStringWithProvider, context)
        {
        }

        public override DatabaseDefinition GetDatabaseDefinition()
        {
            var dd = new DatabaseDefinition();

            Log(LogSeverity.Debug, "Reading table definitions from database.");

            foreach (var schemaAndTableName in GetSchemaAndTableNames())
                dd.AddTable(GetTableDefinition(schemaAndTableName, false));

            Logger.Log(LogSeverity.Debug, "Reading table documentetion from database.", "Reader");

            return dd;
        }

        public override List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            return Executer.ExecuteQuery(@"
SELECT t.table_name tableName, t.owner schemaName
FROM dba_tables t, dba_users u 
WHERE t.owner = u.username
AND EXISTS (SELECT 1 FROM dba_objects o
WHERE o.owner = u.username ) AND default_tablespace not in
('SYSTEM','SYSAUX') and ACCOUNT_STATUS = 'OPEN'").Rows
                .Select(row => new SchemaAndTableName(row.GetAs<string>("SCHEMANAME"), row.GetAs<string>("TABLENAME")))
                .ToList();
        }

        private OracleTableReader12c _tableReader;

        private OracleTableReader12c TableReader => _tableReader ?? (_tableReader = new OracleTableReader12c(Executer));

        public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition)
        {
            var sqlTable = TableReader.GetTableDefinition(schemaAndTableName);

            /*if (fullDefinition)
            {
                new MsSqlPrimaryKeyReader(Executer).
                GetPrimaryKey(sqlTable);
                new MsSqlForeignKeyReader(Executer).GetForeignKeys(sqlTable);
                AddTableDocumentation(sqlTable);
            }

            ColumnDocumentationReader.GetColumnDocumentation(sqlTable);*/

            sqlTable.SchemaAndTableName = GetSchemaAndTableNameAsToStore(sqlTable.SchemaAndTableName, Executer.Generator.Context);

            return sqlTable;
        }
    }
}
