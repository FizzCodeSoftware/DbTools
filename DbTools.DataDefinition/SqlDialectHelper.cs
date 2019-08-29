namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Configuration;

    public static class SqlDialectHelper
    {
        public static SqlDialect GetSqlDialectFromConnectionStringSettings(ConnectionStringSettings connectionStringSettings)
        {
            return GetSqlDialectFromProviderName(connectionStringSettings.ProviderName);
        }

        public static SqlDialect GetSqlDialectFromProviderName(string providerName)
        {
            switch (providerName)
            {
                case "System.Data.SqlClient":
                    return SqlDialect.MsSql;
                case "System.Data.SQLite":
                    return SqlDialect.SqLite;
                case "Oracle.ManagedDataAccess.Client":
                    return SqlDialect.Oracle;
                default:
                    throw new NotImplementedException($"Unmapped connection string provider {providerName}");
            }
        }

        public static string GetProviderNameFromSqlDialect(SqlDialect sqlDialect)
        {
            switch (sqlDialect)
            {
                case SqlDialect.MsSql:
                    return "System.Data.SqlClient";
                case SqlDialect.SqLite:
                    return "System.Data.SQLite";
                case SqlDialect.Oracle:
                    return "Oracle.ManagedDataAccess.Client";
                default:
                    throw new NotImplementedException($"Unmapped sqlDialect {sqlDialect}");
            }
        }
    }
}