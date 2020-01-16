namespace FizzCode.DbTools.Configuration
{
    using System;

    public static class SqlDialectHelper
    {
        public static Type GetSqlDialectTypeFromProviderName(string providerName)
        {
            return providerName switch
            {
                "System.Data.SqlClient" => typeof(IMsSqlDialect),
                "System.Data.SQLite" => typeof(ISqLiteDialect),
                "Oracle.ManagedDataAccess.Client" => typeof(IOracleDialect),
                _ => throw new NotImplementedException($"Unmapped connection string provider {providerName}"),
            };
        }

        public static string GetProviderNameFromSqlDialect(Type sqlDialectType)
        {
            if (typeof(IMsSqlDialect).IsAssignableFrom(sqlDialectType))
                return "System.Data.SqlClient";

            if (typeof(ISqLiteDialect).IsAssignableFrom(sqlDialectType))
                return "System.Data.SQLite";

            if (typeof(IOracleDialect).IsAssignableFrom(sqlDialectType))
                return "Oracle.ManagedDataAccess.Client";

            throw new NotImplementedException($"Unmapped sqlDialect {sqlDialectType.Name}");
        }
    }
}