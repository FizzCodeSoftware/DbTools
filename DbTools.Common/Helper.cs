namespace FizzCode.DbTools.Common
{
    using FizzCode.DbTools.Configuration;
    using Microsoft.Extensions.Configuration;

    public static class Helper
    {
        public static Settings GetDefaultSettings(SqlVersion version, IConfigurationRoot configuration)
        {
            var settings = new Settings();

            var sqlDialectSpecificSettings = new SqlDialectSpecificSettings();

            if (version is IMsSqlDialect)
            {
                sqlDialectSpecificSettings["DefaultSchema"] = "dbo";
            }

            if (version is IOracleDialect)
            {
                sqlDialectSpecificSettings["OracleDatabaseName"] = configuration["oracleDatabaseName"];
            }

            settings.SqlDialectSpecificSettings = sqlDialectSpecificSettings;

            return settings;
        }
    }
}
