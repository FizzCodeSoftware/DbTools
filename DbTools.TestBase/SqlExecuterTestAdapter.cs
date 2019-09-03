namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class SqlExecuterTestAdapter
    {
        private readonly Dictionary<string, (SqlExecuter SqlExecuter, SqlDialect SqlDialect)> sqlExecutersAndDialects = new Dictionary<string, (SqlExecuter, SqlDialect)>();

        private readonly List<DatabaseDefinition> _dds = new List<DatabaseDefinition>();

        public ConnectionStringSettings InitializeAndCheck(SqlDialect sqlDialect, params DatabaseDefinition[] dds)
        {
            TestHelper.CheckProvider(sqlDialect);
            return Initialize(sqlDialect.ToString(), dds);
        }

        public ConnectionStringSettings Initialize(string connectionStringKey, params DatabaseDefinition[] dds)
        {
            _dds.AddRange(dds);
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringKey];

            var sqlDialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);

            if (!sqlExecutersAndDialects.ContainsKey(connectionStringKey))
            {
                var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, TestHelper.GetDefaultTestSettings(sqlDialect));
                var sqlExecuter = SqlExecuterFactory.CreateSqlExecuter(connectionStringSettings, generator);
                sqlExecutersAndDialects.Add(connectionStringKey, (sqlExecuter, sqlDialect));

                var shouldCreate = TestHelper.ShouldRunIntegrationTest(sqlDialect);
                if (shouldCreate)
                {
                    sqlExecuter.InitializeDatabase(false, dds);
                }
            }

            return connectionStringSettings;
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
            var connectionStringSettings = Initialize(connectionStringKey);

            var sqlDialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);

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