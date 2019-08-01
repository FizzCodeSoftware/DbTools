namespace FizzCode.DbTools.TestBase
{
    using System.Configuration;
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
    }
}
