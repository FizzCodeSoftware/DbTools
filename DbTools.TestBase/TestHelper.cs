namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class TestHelper
    {
        private static readonly bool _forceIntegrationTests;
        private static readonly IConfigurationRoot _configuration;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static TestHelper()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            _configuration = Configuration.Load("testconfig", true);
            var forceIntegrationTests = _configuration["forceIntegrationTests"];
            _forceIntegrationTests = forceIntegrationTests == "true";
        }

        public static bool ShouldForceIntegrationTests()
        {
#if INTEGRATION
            return true;
#endif
            return _forceIntegrationTests;
        }

        public static bool ShouldRunIntegrationTest(string providerName)
        {
            if (ShouldForceIntegrationTests())
                return true;

            return providerName switch
            {
                "System.Data.SqlClient" => false,
                "Oracle.ManagedDataAccess.Client" => false,
                _ => true,
            };
        }

        public static bool ShouldRunIntegrationTest(SqlDialect sqlDialect)
        {
            if (ShouldForceIntegrationTests())
                return true;

            return sqlDialect switch
            {
                SqlDialect.MsSql => false,
                SqlDialect.Oracle => false,
                _ => true,
            };
        }

        public static Settings GetDefaultTestSettings(SqlDialect sqlDialect)
        {
            var settings = new Settings();

            var sqlDialectSpecificSettings = new SqlDialectSpecificSettings
            {
                ["DefaultSchema"] = null
            };

            if (sqlDialect == SqlDialect.Oracle)
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                var callerAssemblies = new StackTrace().GetFrames()
                            .Select(f => f.GetMethod().ReflectedType.Assembly).Distinct()
                            .Where(a => a.GetReferencedAssemblies().Any(a2 => a2.FullName == executingAssembly.FullName));
                var initialAssembly = callerAssemblies.Last();

                var assemblyName = initialAssembly.GetName().Name;
                if (assemblyName.StartsWith("FizzCode.DbTools.", StringComparison.CurrentCultureIgnoreCase))
                    assemblyName = assemblyName.Substring("FizzCode.DbTools.".Length);

                var schemaName = assemblyName.Replace(".", "_", StringComparison.CurrentCultureIgnoreCase);

                sqlDialectSpecificSettings["DefaultSchema"] = schemaName;
            }

            if (sqlDialect == SqlDialect.MsSql)
            {
                sqlDialectSpecificSettings["DefaultSchema"] = "dbo";
            }

            if (sqlDialect == SqlDialect.Oracle)
            {
                sqlDialectSpecificSettings["OracleDatabaseName"] = _configuration["Oracle_Database_Name"];
            }

            settings.SqlDialectSpecificSettings = sqlDialectSpecificSettings;

            return settings;
        }

        public static void CheckFeature(SqlDialect sqlDialect, string feature)
        {
            var featureSupport = Features.GetSupport(sqlDialect, feature);
            if (featureSupport.Support == Support.NotSupported)
                Assert.Inconclusive($"Test is skipped, feature {feature} is not supported. ({featureSupport.Description}).");

            if (featureSupport.Support == Support.NotImplementedYet)
                Assert.Inconclusive($"Test is skipped, feature {feature} is not implemented (yet). ({featureSupport.Description}).");
        }

        public static void CheckProvider(SqlDialect sqlDialect)
        {
            CheckAndRegisterInstalledProviders();
            if (!_sqlDialectWithInstalledProviders.Contains(sqlDialect))
                Assert.Inconclusive($"Test is skipped, .Net Framework Data Provider is not installed for {sqlDialect.ToString()} dialect, provier name: {SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect)}");
        }

        private static List<SqlDialect> _sqlDialectWithInstalledProviders;

        private static readonly object syncRoot = new object();

        private static void CheckAndRegisterInstalledProviders()
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("System.Data.SQLite", System.Data.SQLite.SQLiteFactory.Instance);
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("Oracle.ManagedDataAccess.Client", Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);

            lock (syncRoot)
            {
                if (_sqlDialectWithInstalledProviders == null)
                {
                    _sqlDialectWithInstalledProviders = new List<SqlDialect>();

                    var array = Enum.GetValues(typeof(SqlDialect));
                    foreach (var sqlDialect in array.Cast<SqlDialect>())
                    {
                        var providerName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect);
                        if (DbProviderFactories.TryGetFactory(providerName, out _))
                        {
                            _sqlDialectWithInstalledProviders.Add(sqlDialect);
                        }
                    }
                }
            }
        }

        public static Logger CreateLogger()
        {
            var logger = new Logger();

            var configuration = Configuration.Load("testconfig", true);

            var logConfiguration = configuration?.GetSection("Log").Get<LogConfiguration>();

            var iLogger = SerilogConfigurator.CreateLogger(logConfiguration);

            var debugLogger = new DebugLogger();
            debugLogger.Init(iLogger);

            logger.LogEvent += debugLogger.OnLog;

            return logger;
        }
    }
}