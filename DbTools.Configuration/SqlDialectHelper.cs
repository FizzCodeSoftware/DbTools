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
            if (sqlDialectType is IMsSqlDialect)
                return "System.Data.SqlClient";

            if (sqlDialectType is ISqLiteDialect)
                return "System.Data.SQLite";

            if (sqlDialectType is IOracleDialect)
                return "Oracle.ManagedDataAccess.Client";

            throw new NotImplementedException($"Unmapped sqlDialect {sqlDialectType.Name}");
        }
    }
}