namespace FizzCode.DbTools.Common
{
    using FizzCode.DbTools.Configuration;
    using Microsoft.Extensions.Configuration;

    public static class Helper
    {
        public static Settings GetDefaultSettings(SqlVersion version, IConfigurationRoot configuration)
        {
            var settings = new Settings();

            var sqlVersionSpecificSettings = new SqlVersionSpecificSettings();

            if (version is IMsSqlDialect)
            {
                sqlVersionSpecificSettings["DefaultSchema"] = "dbo";
            }

            if (version is IOracleDialect)
            {
                sqlVersionSpecificSettings["OracleDatabaseName"] = configuration["oracleDatabaseName"];
            }

            settings.SqlVersionSpecificSettings = sqlVersionSpecificSettings;

            return settings;
        }
    }
}
