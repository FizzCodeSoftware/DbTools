namespace FizzCode.DbTools.Common
{
    using Microsoft.Extensions.Configuration;

    public static class Helper
    {
        public static Settings GetDefaultSettings(SqlVersion version, IConfigurationRoot configuration)
        {
            var settings = new Settings();

            var sqlDialectSpecificSettings = new SqlDialectSpecificSettings();

            if (version.SqlDialect == SqlDialectX.MsSql)
            {
                sqlDialectSpecificSettings["DefaultSchema"] = "dbo";
            }

            if (version.SqlDialect == SqlDialectX.Oracle)
            {
                sqlDialectSpecificSettings["OracleDatabaseName"] = configuration["oracleDatabaseName"];
            }

            settings.SqlDialectSpecificSettings = sqlDialectSpecificSettings;

            return settings;
        }
    }
}
