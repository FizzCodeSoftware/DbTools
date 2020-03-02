namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class SqlExecuterTestAdapter : ConfigurationBase
    {
        private readonly Dictionary<string, (SqlStatementExecuter SqlExecuter, SqlEngineVersion Version)> sqlExecutersAndDialects = new Dictionary<string, (SqlStatementExecuter, SqlEngineVersion)>();

        private readonly List<DatabaseDefinition> _dds = new List<DatabaseDefinition>();

        public override string ConfigurationFileName => "testconfig";

        public void Check(SqlEngineVersion version)
        {
            TestHelper.CheckProvider(version, ConnectionStrings.All);

            if (!TestHelper.ShouldRunIntegrationTest(version))
                Assert.Inconclusive("Test is skipped, integration tests are not running.");
        }

        public ConnectionStringWithProvider Initialize(string connectionStringKey, params DatabaseDefinition[] dds)
        {
            return Initialize(connectionStringKey, false, dds);
        }

        public ConnectionStringWithProvider InitializeAndCreate(string connectionStringKey, params DatabaseDefinition[] dds)
        {
            return Initialize(connectionStringKey, true, dds);
        }

        private ConnectionStringWithProvider Initialize(string connectionStringKey, bool shouldCreate, params DatabaseDefinition[] dds)
        {
            var connectionStringWithProvider = ConnectionStrings[connectionStringKey];

            if (connectionStringWithProvider == null)
                throw new InvalidOperationException($"No connection string is configured for {connectionStringKey}");

            _dds.AddRange(dds);

            if (!sqlExecutersAndDialects.ContainsKey(connectionStringKey))
            {
                var generator = SqlGeneratorFactory.CreateGenerator(connectionStringWithProvider.SqlEngineVersion, GetContext(connectionStringWithProvider.SqlEngineVersion));
                var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);
                sqlExecutersAndDialects.Add(connectionStringKey, (executer, connectionStringWithProvider.SqlEngineVersion));

                if (shouldCreate && TestHelper.ShouldRunIntegrationTest(connectionStringWithProvider.SqlEngineVersion))
                {
                    executer.InitializeDatabase(false, dds);
                }
            }

            return connectionStringWithProvider;
        }

        private readonly Dictionary<SqlEngineVersion, Context> _contextPerSqlVersion = new Dictionary<SqlEngineVersion, Context>();

        public Context GetContext(SqlEngineVersion version)
        {
            if (!_contextPerSqlVersion.ContainsKey(version))
            {
                var existingContext = _contextPerSqlVersion.Values.FirstOrDefault();
                var existingLogger = existingContext?.Logger;
                var _context = new Context
                {
                    Logger = existingLogger ?? TestHelper.CreateLogger(),
                    Settings = TestHelper.GetDefaultTestSettings(version)
                };

                _contextPerSqlVersion.Add(version, _context);
            }

            return _contextPerSqlVersion[version];
        }

        public void Cleanup()
        {
            var existingContext = _contextPerSqlVersion.Values.FirstOrDefault();
            var existingLogger = existingContext?.Logger;
            existingLogger?.Log(Common.Logger.LogSeverity.Debug, "Cleanup is called.", "SqlExecuterTestAdapter");

            var exceptions = new List<Exception>();
            foreach (var sqlExecuterAndDialect in sqlExecutersAndDialects.Values)
            {
                try
                {
                    var shouldDrop = TestHelper.ShouldRunIntegrationTest(sqlExecuterAndDialect.Version);
                    if (shouldDrop)
                    {
                        sqlExecuterAndDialect.SqlExecuter.CleanupDatabase(true, _dds.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        public string ExecuteNonQuery(string connectionStringKey, string query)
        {
            var connectionStringWithProvider = Initialize(connectionStringKey);

            if (!TestHelper.ShouldRunIntegrationTest(connectionStringWithProvider.SqlEngineVersion))
                return "Query execution is skipped, integration tests are not running.";

            sqlExecutersAndDialects[connectionStringKey].SqlExecuter.ExecuteNonQuery(query);
            return null;
        }

        public SqlStatementExecuter GetExecuter(string connectionStringKey)
        {
            return sqlExecutersAndDialects[connectionStringKey].SqlExecuter;
        }
    }
}