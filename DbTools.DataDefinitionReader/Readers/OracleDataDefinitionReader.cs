namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class OracleDataDefinitionReader : GenericDataDefinitionReader
    {
        public OracleDataDefinitionReader(SqlExecuter sqlExecuter) : base(sqlExecuter)
        {
        }

        protected override SqlDialect SqlDialect => SqlDialect.Oracle;

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
SELECT table_name tableName, owner schemaName FROM information_schema.TABLES").Rows
                .Select(row => new SchemaAndTableName(row.GetAs<string>("schemaName"), row.GetAs<string>("tableName")))
                .ToList();
        }

        public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition)
        {
            throw new NotImplementedException();
        }
    }
}
