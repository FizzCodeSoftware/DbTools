namespace FizzCode.DbTools.Configuration
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

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

        public ConnectionStringFields GetKnownConnectionStringFields()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return null;

            var values = ConnectionString
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim());

            var result = new ConnectionStringFields();

            foreach (var v in values)
            {
                var idx = v.IndexOf("=");
                if (idx == -1)
                    continue;

                var name = v.Substring(0, idx).ToLowerInvariant();
                var value = v.Substring(idx + 1);
                switch (name)
                {
                    case "server":
                    case "data source":
                        result.Server = value;
                        // todo: support oracle's complex Data Source format:
                        /*if (KnownProvider == Configuration.KnownProvider.Oracle)
                        {
                            // Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=MyHost)(PORT=MyPort))(CONNECT_DATA=(SERVICE_NAME=MyOracleSID)));
                        }*/
                        break;
                    case "database":
                    case "initial catalog":
                        result.Database = value;
                        break;
                    case "user id":
                    case "uid":
                        result.UserId = value;
                        break;
                    case "port":
                        if (int.TryParse(value, out var port))
                            result.Port = port;
                        break;
                    case "integratedsecurity":
                    case "integrated security":
                        result.IntegratedSecurity = string.Equals(value, "yes", StringComparison.InvariantCultureIgnoreCase)
                            || string.Equals(value, "sspi", StringComparison.InvariantCultureIgnoreCase);
                        break;
                }
            }
            return result;
        }
    }
}