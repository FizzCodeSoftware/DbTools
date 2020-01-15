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
    using FizzCode.DbTools.Configuration;
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

        public static bool ShouldRunIntegrationTest(SqlVersion version)
        {
            return ShouldRunIntegrationTest(version.SqlDialect);
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

        private static bool ShouldRunIntegrationTest(SqlDialectX sqlDialect)
        {
            if (ShouldForceIntegrationTests())
                return true;

            return sqlDialect switch
            {
                SqlDialectX.MsSql => false,
                SqlDialectX.Oracle => false,
                _ => true,
            };
        }

        public static Settings GetDefaultTestSettings(SqlVersion version)
        {
            var settings = Helper.GetDefaultSettings(version, _configuration);

            if (version.SqlDialect == SqlDialectX.Oracle)
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

                settings.SqlDialectSpecificSettings["DefaultSchema"] = schemaName;
            }

            return settings;
        }

        public static void CheckFeature(SqlVersion version, string feature)
        {
            var featureSupport = Features.GetSupport(version, feature);
            if (featureSupport.Support == Support.NotSupported)
                Assert.Inconclusive($"Test is skipped, feature {feature} is not supported. ({featureSupport.Description}).");

            if (featureSupport.Support == Support.NotImplementedYet)
                Assert.Inconclusive($"Test is skipped, feature {feature} is not implemented (yet). ({featureSupport.Description}).");
        }

        public static void CheckProvider(SqlDialectX sqlDialect, IEnumerable<ConnectionStringWithProvider> connectionStringWithProviders)
        {
            RegisterProviders();
            var usedSqlDialects = GetSqlDialectsWithConfiguredConnectionStrting(connectionStringWithProviders);
            if (!usedSqlDialects.Contains(sqlDialect))
                Assert.Inconclusive($"Test is skipped, .Net Framework Data Provider is not usabe for {sqlDialect.ToString()} dialect, provider name: {SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect)}. No valid connection string is configured.");
        }

        private static List<SqlDialectX> _sqlDialectsWithConfiguredConnectionStrting;

        private static List<SqlDialectX> GetSqlDialectsWithConfiguredConnectionStrting(IEnumerable<ConnectionStringWithProvider> connectionStringCollection)
        {
            if (_sqlDialectsWithConfiguredConnectionStrting == null)
            {
                _sqlDialectsWithConfiguredConnectionStrting = new List<SqlDialectX>();
                foreach (var connectionStringWithProvider in connectionStringCollection)
                {
                    if (!string.IsNullOrEmpty(connectionStringWithProvider.ConnectionString))
                        _sqlDialectsWithConfiguredConnectionStrting.Add(SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName));
                }
            }

            return _sqlDialectsWithConfiguredConnectionStrting;
        }

        private static bool _areDbProviderFactoriesRegistered;

        private static void RegisterProviders()
        {
            if (!_areDbProviderFactoriesRegistered)
            {
                DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
                DbProviderFactories.RegisterFactory("System.Data.SQLite", System.Data.SQLite.SQLiteFactory.Instance);
                DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
                DbProviderFactories.RegisterFactory("Oracle.ManagedDataAccess.Client", Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);

                _areDbProviderFactoriesRegistered = true;
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