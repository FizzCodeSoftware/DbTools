namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class TestHelper
    {
        public static bool ShouldForceIntegrationTests()
        {
#if INTEGRATION
            return true;
#endif
            var setting = ConfigurationManager.AppSettings["forceIntegrationTests"];
            return setting == "true";
        }

        public static bool ShouldRunIntegrationTest(string providerName)
        {
            if (ShouldForceIntegrationTests())
                return true;

            switch (providerName)
            {
                case "System.Data.SqlClient":
                    return false;
                default:
                    return true;
            }
        }

        public static bool ShouldRunIntegrationTest(SqlDialect sqlDialect)
        {
            if (ShouldForceIntegrationTests())
                return true;

            switch (sqlDialect)
            {
                case SqlDialect.MsSql:
                    return false;
                default:
                    return true;
            }
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
                if (assemblyName.StartsWith("FizzCode.DbTools."))
                    assemblyName = assemblyName.Substring("FizzCode.DbTools.".Length);

                var schemaName = assemblyName.Replace(".", "_");

                sqlDialectSpecificSettings["DefaultSchema"] = schemaName;
            }

            if (sqlDialect == SqlDialect.MsSql)
            {
                sqlDialectSpecificSettings["DefaultSchema"] = "dbo";
            }

            settings.SqlDialectSpecificSettings = sqlDialectSpecificSettings;

            return settings;
        }

        public static void CheckFeature(SqlDialect sqlDialect, string feature)
        {
            var featureSupport = Features.Instance[sqlDialect, feature];
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

        public static List<SqlDialect> _sqlDialectWithInstalledProviders;

        private static readonly object syncRoot = new object();

        private static void CheckAndRegisterInstalledProviders()
        {
            lock (syncRoot)
            {
                if (_sqlDialectWithInstalledProviders == null)
                {
                    _sqlDialectWithInstalledProviders = new List<SqlDialect>();

                    var array = Enum.GetValues(typeof(SqlDialect));
                    foreach (var sqlDialect in array.Cast<SqlDialect>())
                    {
                        var providerName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect);

                        try
                        {
                            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect));
                        }
                        catch (ConfigurationException ex) when (ex.BareMessage == "Failed to find or load the registered .Net Framework Data Provider.")
                        {
                            break;
                        }

                        _sqlDialectWithInstalledProviders.Add(sqlDialect);
                    }
                }
            }
        }
    }
}
