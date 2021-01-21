namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.LightWeight.AdoNet;
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

        public NamedConnectionString Initialize(string connectionStringKey, params DatabaseDefinition[] dds)
        {
            return Initialize(connectionStringKey, false, dds);
        }

        public NamedConnectionString InitializeAndCreate(string connectionStringKey, params DatabaseDefinition[] dds)
        {
            return Initialize(connectionStringKey, true, dds);
        }

        private NamedConnectionString Initialize(string connectionStringKey, bool shouldCreate, params DatabaseDefinition[] dds)
        {
            var connectionString = ConnectionStrings[connectionStringKey];

            if (connectionString == null)
                throw new InvalidOperationException($"No connection string is configured for {connectionStringKey}");

            var sqlEngineVersion = connectionString.GetSqlEngineVersion();

            _dds.AddRange(dds);

            if (!sqlExecutersAndDialects.ContainsKey(connectionStringKey))
            {
                var generator = SqlGeneratorFactory.CreateGenerator(sqlEngineVersion, GetContext(sqlEngineVersion));
                var executer = SqlExecuterFactory.CreateSqlExecuter(connectionString, generator);
                sqlExecutersAndDialects.Add(connectionStringKey, (executer, sqlEngineVersion));

                if (shouldCreate && TestHelper.ShouldRunIntegrationTest(sqlEngineVersion))
                {
                    executer.InitializeDatabase(false, dds);
                }
            }

            return connectionString;
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
            var connectionString = Initialize(connectionStringKey);

            var sqlEngineVersion = connectionString.GetSqlEngineVersion();

            if (!TestHelper.ShouldRunIntegrationTest(sqlEngineVersion))
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