namespace FizzCode.DbTools.TestBase
{
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;

    public static class Helper
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

            SqlDialectSpecificSettings sqlDialectSpecificSettings = null;
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

                sqlDialectSpecificSettings = new SqlDialectSpecificSettings
                {
                    { "DefaultSchema", schemaName }
                };
            }

            if (sqlDialect == SqlDialect.MsSql)
            {
                sqlDialectSpecificSettings = new SqlDialectSpecificSettings
                    {
                        { "DefaultSchema", "dbo" }
                    };
            }

            settings.SqlDialectSpecificSettings = sqlDialectSpecificSettings;

            return settings;
        }
    }
}
