namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class SqlExecuterTestAdapter : ConfigurationBase
    {
        private readonly Dictionary<string, (SqlExecuter SqlExecuter, SqlVersion Version)> sqlExecutersAndDialects = new Dictionary<string, (SqlExecuter, SqlVersion)>();

        private readonly List<DatabaseDefinition> _dds = new List<DatabaseDefinition>();

        public override string ConfigurationFileName => "testconfig";

        public void Check(SqlVersion version)
        {
            TestHelper.CheckProvider(version.SqlDialect, ConnectionStrings.All);

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
            _dds.AddRange(dds);

            var connectionStringWithProvider = ConnectionStrings[connectionStringKey];

            var sqlDialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);
            var version = SqlEngines.GetLatestVersion(sqlDialect);

            if (!sqlExecutersAndDialects.ContainsKey(connectionStringKey))
            {
                var generator = SqlGeneratorFactory.CreateGenerator(version, GetContext(version));
                var sqlExecuter = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);
                sqlExecutersAndDialects.Add(connectionStringKey, (sqlExecuter, version));

                if (shouldCreate && TestHelper.ShouldRunIntegrationTest(version))
                {
                    sqlExecuter.InitializeDatabase(false, dds);
                }
            }

            return connectionStringWithProvider;
        }

        private readonly Dictionary<SqlVersion, Context> _contextPerSqlVersion = new Dictionary<SqlVersion, Context>();

        public Context GetContext(SqlVersion version)
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
                        sqlExecuterAndDialect.SqlExecuter.CleanupDatabase(_dds.ToArray());
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

            var sqlDialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);
            var version = SqlEngines.GetLatestVersion(sqlDialect);

            if (!TestHelper.ShouldRunIntegrationTest(version))
                return "Query execution is skipped, integration tests are not running.";

            sqlExecutersAndDialects[connectionStringKey].SqlExecuter.ExecuteNonQuery(query);
            return null;
        }

        public SqlExecuter GetExecuter(string connectionStringKey)
        {
            return sqlExecutersAndDialects[connectionStringKey].SqlExecuter;
        }
    }
}