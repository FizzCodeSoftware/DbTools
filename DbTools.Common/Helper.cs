namespace FizzCode.DbTools.Common
{
    public static class Helper
    {
        public static Settings GetDefaultTestSettings(SqlDialect sqlDialect)
        {
            var settings = new Settings();

            SqlDialectSpecificSettings sqlDialectSpecificSettings = null;
            if (sqlDialect == SqlDialect.Oracle)
            {
                // TODO check if always correct?
                var assemblyName = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;
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
