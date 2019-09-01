namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class OracleExecuter : SqlExecuter
    {
        protected override SqlDialect SqlDialect => SqlDialect.Oracle;

        public OracleExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator) : base(connectionStringSettings, sqlGenerator)
        {
        }

        public override DbConnection OpenConnectionMaster()
        {
            return OpenConnection();
        }

        public override string GetDatabase(DbConnectionStringBuilder builder)
        {
            throw new NotImplementedException("Oracle executer does not handle database name.");
        }

        public override void InitializeDatabase(bool dropIfExists, params DatabaseDefinition[] dds)
        {
            var defaultSchema = GetSettings().SqlDialectSpecificSettings.GetAs<string>("DefaultSchema");

            if (dropIfExists && CheckIfUserExists(defaultSchema))
                CleanupDatabase(dds);

            ExecuteQuery($"CREATE USER \"{defaultSchema}\" IDENTIFIED BY sa123");
            ExecuteQuery($"GRANT CONNECT, DBA TO \"{defaultSchema}\"");
            ExecuteQuery($"GRANT CREATE SESSION TO \"{defaultSchema}\"");
            ExecuteQuery($"GRANT UNLIMITED TABLESPACE TO \"{defaultSchema}\"");

            /*var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            var connect_identifier = "//" + builder.ValueOfKey("DATA SOURCE");
            ExecuteQuery($"CONNECT {defaultSchema}/sa123@{connect_identifier}");*/

            ExecuteQuery($"ALTER SESSION SET current_schema = \"{defaultSchema}\"");
        }

        public bool CheckIfUserExists(string userName)
        {
            var result = ExecuteScalar(((OracleGenerator)Generator).IfExists("dba_users", "username", userName));
            return (decimal)result == 0;
        }

        public override void CleanupDatabase(params DatabaseDefinition[] dds)
        {
            var defaultSchema = GetSettings().SqlDialectSpecificSettings.GetAs<string>("DefaultSchema");
            // TODO - DROP ALL Schemas - in current DD

            var currentUser = ExecuteQuery("select user from dual").Rows[0].GetAs<string>("USER");

            ExecuteQuery($"ALTER SESSION SET current_schema = {currentUser}");
            ExecuteQuery($"DROP USER \"{defaultSchema}\" CASCADE");
        }

        private readonly OracleDbCommandPreparer _oracleSqlCommandPreparer = new OracleDbCommandPreparer();

        public override DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var dbCommand = base.PrepareSqlCommand(sqlStatementWithParameters);
            return _oracleSqlCommandPreparer.PrepareSqlCommand(dbCommand);
        }

        protected override void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters)
        {
            ExecuteQuery(sqlStatementWithParameters);
        }
    }
}