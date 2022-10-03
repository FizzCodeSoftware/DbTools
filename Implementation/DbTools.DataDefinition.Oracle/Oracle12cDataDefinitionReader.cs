namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.DataDefinitionReader;
    using FizzCode.DbTools.SqlExecuter.Oracle;
    using FizzCode.LightWeight.AdoNet;

    public class Oracle12cDataDefinitionReader : GenericDataDefinitionReader
    {
        public Oracle12cDataDefinitionReader(NamedConnectionString connectionString, ContextWithLogger context, ISchemaNamesToRead schemaNames)
            : base(new Oracle12cExecuter(context, connectionString, new Oracle12cGenerator(context)), schemaNames)
        {
        }

        public override DatabaseDefinition GetDatabaseDefinition()
        {
            var dd = new DatabaseDefinition(OracleVersion.Oracle12c, GenericVersion.Generic1);

            Log(LogSeverity.Debug, "Reading table definitions from database.");

            foreach (var schemaAndTableName in GetSchemaAndTableNames())
                dd.AddTable(GetTableDefinition(schemaAndTableName, false));

            Log(LogSeverity.Debug, "Reading table identities from database.");
            new OracleIdentityReader12c(Executer, SchemaNames).GetIdentity(dd);
            Log(LogSeverity.Debug, "Reading table primary keys from database.");
            new OraclePrimaryKeyReader12c(Executer, SchemaNames).GetPrimaryKey(dd);
            Log(LogSeverity.Debug, "Reading table foreign keys including unique constrints from database.", "Reader");
            new OracleForeignKeyReader12c(Executer, SchemaNames).GetForeignKeysAndUniqueConstrainsts(dd);
            Log(LogSeverity.Debug, "Reading indexes from database.");
            new OracleIndexReader12c(Executer, SchemaNames).GetIndexes(dd);

            return dd;
        }

        public override List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            return new OracleTablesReader(Executer, SchemaNames).GetSchemaAndTableNames();
        }

        private OracleTableReader12c _tableReader;

        private OracleTableReader12c TableReader => _tableReader ??= new OracleTableReader12c(Executer, SchemaNames);

        public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition)
        {
            var sqlTable = TableReader.GetTableDefinition(schemaAndTableName);

            if (fullDefinition)
            {
                new OraclePrimaryKeyReader12c(Executer, SchemaNames).
                GetPrimaryKey(sqlTable);
                new OracleForeignKeyReader12c(Executer, SchemaNames).GetForeignKeys(sqlTable);
                // TODO
                //AddTableDocumentation(sqlTable);
            }
            // TODO 
            // ColumnDocumentationReader.GetColumnDocumentation(sqlTable);

            sqlTable.SchemaAndTableName = GetSchemaAndTableNameAsToStore(sqlTable.SchemaAndTableName, Executer.Context);

            return sqlTable;
        }

        public override List<SchemaAndTableName> GetViews()
        {
            throw new System.NotImplementedException();
        }

        public override SqlView GetViewDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition)
        {
            throw new System.NotImplementedException();
        }
    }
}
