namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Configuration;
    using System.Data.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class OracleExecuter : SqlExecuter
    {
        protected override SqlDialect SqlDialect => SqlDialect.Oracle;

        private readonly SqlDialectSpecificSettings _settings;

        public OracleExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator, SqlDialectSpecificSettings settings) : base(connectionStringSettings, sqlGenerator)
        {
            _settings = settings;
        }

        public override DbConnection OpenConnectionMaster()
        {
            return OpenConnection();
        }

        public override string GetDatabase(DbConnectionStringBuilder builder)
        {
            var oracleDatabaseNameKey = ConnectionStringSettings.Name + "_Database_Name";
            var oracleDatabaseName = ConfigurationManager.AppSettings[oracleDatabaseNameKey];
            return oracleDatabaseName;
        }

        public override void InitializeDatabase()
        {
            var defaultSchema = _settings.GetAs<string>("DefaultSchema");

            ExecuteQuery($"CREATE USER {defaultSchema} IDENTIFIED BY sa123");
            ExecuteQuery($"GRANT CONNECT, DBA TO {defaultSchema}");
            ExecuteQuery($"GRANT CREATE SESSION TO {defaultSchema}");
            ExecuteQuery($"GRANT UNLIMITED TABLESPACE TO {defaultSchema}");

            /*var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            var connect_identifier = "//" + builder.ValueOfKey("DATA SOURCE");
            ExecuteQuery($"CONNECT {defaultSchema}/sa123@{connect_identifier}");*/

            ExecuteQuery($"ALTER SESSION SET current_schema = {defaultSchema}");
        }

        public override void CleanupDatabase(params DatabaseDefinition[] dds)
        {
            var defaultSchema = _settings.GetAs<string>("DefaultSchema");
            // TODO - DROP ALL Schemas - in current DD

            var currentUser = ExecuteQuery("select user from dual").Rows[0].GetAs<string>("USER");

            ExecuteQuery($"ALTER SESSION SET current_schema = {currentUser}");
            ExecuteQuery($"DROP USER {defaultSchema} CASCADE");
        }

        protected override void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters)
        {
            ExecuteQuery(sqlStatementWithParameters);
        }
    }
}