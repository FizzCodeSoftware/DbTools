namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;

    public static class SqlDialectHelper
    {
        public static SqlDialectX GetSqlDialectFromProviderName(string providerName)
        {
            return providerName switch
            {
                "System.Data.SqlClient" => SqlDialectX.MsSql,
                "System.Data.SQLite" => SqlDialectX.SqLite,
                "Oracle.ManagedDataAccess.Client" => SqlDialectX.Oracle,
                _ => throw new NotImplementedException($"Unmapped connection string provider {providerName}"),
            };
        }

        public static string GetProviderNameFromSqlDialect(SqlDialectX sqlDialect)
        {
            return sqlDialect switch
            {
                SqlDialectX.MsSql => "System.Data.SqlClient",
                SqlDialectX.SqLite => "System.Data.SQLite",
                SqlDialectX.Oracle => "Oracle.ManagedDataAccess.Client",
                _ => throw new NotImplementedException($"Unmapped sqlDialect {sqlDialect}"),
            };
        }
    }
}