namespace FizzCode.DbTools.Common
{
    using Microsoft.Extensions.Configuration;

    public static class Helper
    {
        public static Settings GetDefaultSettings(SqlDialect sqlDialect, IConfigurationRoot configuration)
        {
            var settings = new Settings();

            var sqlDialectSpecificSettings = new SqlDialectSpecificSettings();

            if (sqlDialect == SqlDialect.MsSql)
            {
                sqlDialectSpecificSettings["DefaultSchema"] = "dbo";
            }

            if (sqlDialect == SqlDialect.Oracle)
            {
                sqlDialectSpecificSettings["OracleDatabaseName"] = configuration["oracleDatabaseName"];
            }

            settings.SqlDialectSpecificSettings = sqlDialectSpecificSettings;

            return settings;
        }

        public static Settings GetDefaultSettings(SqlDialect sqlDialect)
        {
            var settings = new Settings();

            var sqlDialectSpecificSettings = new SqlDialectSpecificSettings();

            if (sqlDialect == SqlDialect.MsSql)
            {
                sqlDialectSpecificSettings["DefaultSchema"] = "dbo";
            }

            settings.SqlDialectSpecificSettings = sqlDialectSpecificSettings;

            return settings;
        }
    }
}
