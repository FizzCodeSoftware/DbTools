namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;

    public static class SqlDialectHelper
    {
        public static SqlDialect GetSqlDialectFromProviderName(string providerName)
        {
            return providerName switch
            {
                "System.Data.SqlClient" => SqlDialect.MsSql,
                "System.Data.SQLite" => SqlDialect.SqLite,
                "Oracle.ManagedDataAccess.Client" => SqlDialect.Oracle,
                _ => throw new NotImplementedException($"Unmapped connection string provider {providerName}"),
            };
        }

        public static string GetProviderNameFromSqlDialect(SqlDialect sqlDialect)
        {
            return sqlDialect switch
            {
                SqlDialect.MsSql => "System.Data.SqlClient",
                SqlDialect.SqLite => "System.Data.SQLite",
                SqlDialect.Oracle => "Oracle.ManagedDataAccess.Client",
                _ => throw new NotImplementedException($"Unmapped sqlDialect {sqlDialect}"),
            };
        }
    }
}