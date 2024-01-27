namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.Interfaces;
    using FizzCode.LightWeight.AdoNet;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class SqlExecuterTestAdapter : ConfigurationBase
    {
        private readonly Dictionary<string, (ISqlStatementExecuter SqlExecuter, SqlEngineVersion Version)> sqlExecutersAndDialects = new();

        private readonly Dictionary<SqlConnectionKeyAndDatabaseDefinitionTypeAsKey, DatabaseDefinition> _dds = new();

        public override string ConfigurationFileName => "testconfig";

        private readonly ISqlExecuterFactory _sqlExecuterFactory;
        private readonly IFactoryContainer _root;

        public SqlExecuterTestAdapter()
        {
            _root = new TestFactoryContainer();
            _sqlExecuterFactory = _root.Get<ISqlExecuterFactory>();
    }

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

            foreach(var dd in dds)
            {
                var key = new SqlConnectionKeyAndDatabaseDefinitionTypeAsKey(connectionStringKey, dd);
                if (!_dds.ContainsKey(key))
                    _dds.Add(key, dd);
            }

            if (!sqlExecutersAndDialects.ContainsKey(connectionStringKey))
            {
                //var generator = _sqlGeneratorFactory.CreateSqlGenerator(sqlEngineVersion, GetContext(sqlEngineVersion));
                var executer = _sqlExecuterFactory.CreateSqlExecuter(connectionString);
                sqlExecutersAndDialects.Add(connectionStringKey, (executer, sqlEngineVersion));

                if (shouldCreate && TestHelper.ShouldRunIntegrationTest(sqlEngineVersion))
                {
                    executer.InitializeDatabase(false, dds);
                }
            }

            return connectionString;
        }

        public void Cleanup()
        {
            var logger = _root.Get<Logger>();
            logger.Log(LogSeverity.Debug, "Cleanup is called.", "SqlExecuterTestAdapter");

            var exceptions = new List<Exception>();
            foreach (var sqlExecuterAndDialect in sqlExecutersAndDialects.Values)
            {
                logger.Log(LogSeverity.Debug, $"Cleanup is called for {sqlExecuterAndDialect.Version}", "SqlExecuterTestAdapter");
                try
                {
                    var shouldDrop = TestHelper.ShouldRunIntegrationTest(sqlExecuterAndDialect.Version);
                    if (shouldDrop)
                    {
                        var dds = _dds.Where(e => e.Key == new SqlConnectionKeyAndDatabaseDefinitionTypeAsKey(sqlExecuterAndDialect.Version.UniqueName, e.Value)).Select(e => e.Value).ToArray();
                        sqlExecuterAndDialect.SqlExecuter.CleanupDatabase(true, dds);
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

        public ISqlStatementExecuter GetExecuter(string connectionStringKey)
        {
            return sqlExecutersAndDialects[connectionStringKey].SqlExecuter;
        }
    }
}