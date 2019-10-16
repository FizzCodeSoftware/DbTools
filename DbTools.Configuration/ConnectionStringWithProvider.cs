namespace FizzCode.DbTools.Configuration
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("{Name}, {ProviderName}, {ConnectionString}")]
    public class ConnectionStringWithProvider
    {
        public string Name { get; set; }
        public string ProviderName { get => _providerName; set { _providerName = value; UpdateKnownProvider(); } }
        public string ConnectionString { get; set; }
        public KnownProvider? KnownProvider { get; private set; }

        private string _providerName { get; set; }

        public ConnectionStringWithProvider(string name, string providerName, string connectionString)
        {
            Name = name;
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        public override string ToString()
        {
            // do not include ConnectionString here due to security reasons
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", Name, ProviderName);
        }

        private void UpdateKnownProvider()
        {
            KnownProvider = null;

            if (string.Equals(ProviderName, "System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase))
                KnownProvider = Configuration.KnownProvider.MsSql;
            else if (string.Equals(ProviderName, "System.Data.SQLite", StringComparison.InvariantCultureIgnoreCase))
                KnownProvider = Configuration.KnownProvider.SqLite;
            else if (string.Equals(ProviderName, "Oracle.ManagedDataAccess.Client", StringComparison.InvariantCultureIgnoreCase))
                KnownProvider = Configuration.KnownProvider.Oracle;
            else if (string.Equals(ProviderName, "MySql.Data.MySqlClient", StringComparison.InvariantCultureIgnoreCase))
                KnownProvider = Configuration.KnownProvider.MySql;
            else if (string.Equals(ProviderName, "Npgsql", StringComparison.InvariantCultureIgnoreCase))
                KnownProvider = Configuration.KnownProvider.PgSql;
        }
    }
}