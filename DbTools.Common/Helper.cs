namespace FizzCode.DbTools.Common
{
    public static class Helper
    {
        public static Settings GetDefaultSettings(SqlDialect sqlDialect)
        {
            var settings = new Settings();

            SqlDialectSpecificSettings sqlDialectSpecificSettings = null;

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
