using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                sqlDialectSpecificSettings = new SqlDialectSpecificSettings
                {
                    { "DefaultSchema", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.TrimStart("DbTools.".ToCharArray()).Replace(".", "_") }
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
