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
        private readonly Dictionary<string, (SqlExecuter SqlExecuter, SqlDialect SqlDialect)> sqlExecutersAndDialects = new Dictionary<string, (SqlExecuter, SqlDialect)>();

        private readonly List<DatabaseDefinition> _dds = new List<DatabaseDefinition>();

        public override string ConfigurationFileName => "testconfig";

        public void Check(SqlDialect sqlDialect)
        {
            TestHelper.CheckProvider(sqlDialect, ConnectionStrings.All);

            if (!TestHelper.ShouldRunIntegrationTest(sqlDialect))
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

            if (!sqlExecutersAndDialects.ContainsKey(connectionStringKey))
            {
                var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, GetContext(sqlDialect));
                var sqlExecuter = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);
                sqlExecutersAndDialects.Add(connectionStringKey, (sqlExecuter, sqlDialect));

                if (shouldCreate && TestHelper.ShouldRunIntegrationTest(sqlDialect))
                {
                    sqlExecuter.InitializeDatabase(false, dds);
                }
            }

            return connectionStringWithProvider;
        }

        private readonly Dictionary<SqlDialect, Context> _contextPerSqlDialect = new Dictionary<SqlDialect, Context>();

        public Context GetContext(SqlDialect sqlDialect)
        {
            if (!_contextPerSqlDialect.ContainsKey(sqlDialect))
            {
                var existingContext = _contextPerSqlDialect.Values.FirstOrDefault();
                var existingLogger = existingContext?.Logger;
                var _context = new Context
                {
                    Logger = existingLogger ?? TestHelper.CreateLogger(),
                    Settings = TestHelper.GetDefaultTestSettings(sqlDialect)
                };

                _contextPerSqlDialect.Add(sqlDialect, _context);
            }

            return _contextPerSqlDialect[sqlDialect];
        }

        public void Cleanup()
        {
            var exceptions = new List<Exception>();
            foreach (var sqlExecuterAndDialect in sqlExecutersAndDialects.Values)
            {
                try
                {
                    var shouldDrop = TestHelper.ShouldRunIntegrationTest(sqlExecuterAndDialect.SqlDialect);
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

            if (!TestHelper.ShouldRunIntegrationTest(sqlDialect))
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