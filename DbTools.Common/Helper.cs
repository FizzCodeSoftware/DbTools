namespace FizzCode.DbTools.Common
{
    using FizzCode.DbTools.Configuration;
    using Microsoft.Extensions.Configuration;

    public static class Helper
    {
        public static Settings GetDefaultSettings(SqlEngineVersion version, IConfigurationRoot configuration)
        {
            var settings = new Settings();

            var sqlVersionSpecificSettings = new SqlVersionSpecificSettings();

            if (version is MsSqlVersion)
            {
                sqlVersionSpecificSettings["DefaultSchema"] = "dbo";
            }

            if (version is OracleVersion)
            {
                sqlVersionSpecificSettings["OracleDatabaseName"] = configuration["oracleDatabaseName"];
            }

            settings.SqlVersionSpecificSettings = sqlVersionSpecificSettings;

            return settings;
        }

        public static Settings GetDefaultSettings(SqlEngineVersion version)
        {
            var settings = new Settings();

            var sqlVersionSpecificSettings = new SqlVersionSpecificSettings();

            if (version is MsSqlVersion)
            {
                sqlVersionSpecificSettings["DefaultSchema"] = "dbo";
            }

            settings.SqlVersionSpecificSettings = sqlVersionSpecificSettings;

            return settings;
        }
    }
}
