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
        public string ProviderName { get => _providerName; set { _providerName = value; UpdateEngine(); } }
        public string ConnectionString { get; set; }
        public string Version { get => _version; set { _version = value; UpdateEngine(); } }

        public SqlEngine? SqlEngine { get; private set; }
        public SqlEngineVersion SqlEngineVersion { get; set; }

        private string _providerName;
        private string _version;

        public ConnectionStringWithProvider(string name, string providerName, string connectionString, string version)
        {
            Name = name;
            ConnectionString = connectionString;
            _providerName = providerName;
            _version = version;
            UpdateEngine();
        }

        public override string ToString()
        {
            // do not include ConnectionString here due to security reasons
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", Name, ProviderName);
        }

        private void UpdateEngine()
        {
            SqlEngine = null;
            SqlEngineVersion = null;

            var versionsByType = SqlEngineVersions.AllVersions.GroupBy(x => x.GetType());
            foreach (var group in versionsByType)
            {
                var matches = group
                    .Where(version => string.Equals(ProviderName, version.ProviderName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                if (matches.Count > 0)
                {
                    SqlEngine = matches[0].Engine;
                    SqlEngineVersion = matches.Find(x => string.Equals(x.VersionString, Version, StringComparison.InvariantCultureIgnoreCase));
                    break;
                }
            }
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
                var idx = v.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                if (idx == -1)
                    continue;

                var name = v.Substring(0, idx).ToUpperInvariant();
                var value = v.Substring(idx + 1);
                switch (name)
                {
                    case "SERVER":
                    case "DATA SOURCE":
                        result.Server = value;
                        // todo: support oracle's complex Data Source format:
                        /*if (KnownProvider == Configuration.KnownProvider.Oracle)
                        {
                            // Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=MyHost)(PORT=MyPort))(CONNECT_DATA=(SERVICE_NAME=MyOracleSID)));
                        }*/
                        break;
                    case "DATABASE":
                    case "INITIAL CATALOG":
                        result.Database = value;
                        break;
                    case "USER ID":
                    case "UID":
                        result.UserId = value;
                        break;
                    case "PORT":
                        if (int.TryParse(value, out var port))
                            result.Port = port;
                        break;
                    case "INTEGRATEDSECURITY":
                    case "INTEGRATED SECURITY":
                        result.IntegratedSecurity = string.Equals(value, "yes", StringComparison.InvariantCultureIgnoreCase)
                            || string.Equals(value, "sspi", StringComparison.InvariantCultureIgnoreCase);
                        break;
                }
            }

            return result;
        }

        public string GetFriendlyProviderName()
        {
            if (SqlEngine != null)
                return SqlEngine.ToString();

            return ProviderName;
        }

        public bool IsEscaped(string identifier)
        {
            if (SqlEngine == null)
                throw new NotSupportedException();

            switch (SqlEngine)
            {
                case Configuration.SqlEngine.MsSql:
                    return identifier.StartsWith('[') && identifier.EndsWith(']');
                case Configuration.SqlEngine.SqLite:
                case Configuration.SqlEngine.PostgreSql:
                case Configuration.SqlEngine.OracleSql:
                    return identifier.StartsWith('\"') && identifier.EndsWith('\"');
                case Configuration.SqlEngine.MySql:
                    return identifier.StartsWith('`') && identifier.EndsWith('`');
            }

            throw new NotSupportedException();
        }

        public string Escape(string dbObject, string schema = null)
        {
            if (SqlEngine == null)
                throw new NotSupportedException();

            switch (SqlEngine)
            {
                case Configuration.SqlEngine.MsSql:
                case Configuration.SqlEngine.SqLite:
                case Configuration.SqlEngine.MySql:
                case Configuration.SqlEngine.PostgreSql:
                case Configuration.SqlEngine.OracleSql:
                    if (!string.IsNullOrEmpty(schema))
                        return EscapeIdentifier(schema) + "." + EscapeIdentifier(dbObject);

                    return EscapeIdentifier(dbObject);
            }

            throw new NotSupportedException();
        }

        private string EscapeIdentifier(string identifier)
        {
            switch (SqlEngine)
            {
                case Configuration.SqlEngine.MsSql:
                    return identifier.StartsWith('[') && identifier.EndsWith(']')
                         ? identifier
                         : "[" + identifier + "]";
                case Configuration.SqlEngine.SqLite:
                case Configuration.SqlEngine.PostgreSql:
                case Configuration.SqlEngine.OracleSql:
                    return identifier.StartsWith('\"') && identifier.EndsWith('\"')
                        ? identifier
                        : "\"" + identifier + "\"";
                case Configuration.SqlEngine.MySql:
                    return identifier.StartsWith('`') && identifier.EndsWith('`')
                        ? identifier
                        : "`" + identifier + "`";
            }

            throw new NotSupportedException();
        }

        public string Unescape(string identifier)
        {
            if (SqlEngine == null)
                throw new NotSupportedException();

            switch (SqlEngine)
            {
                case Configuration.SqlEngine.MsSql:
                    return identifier
                        .Replace("[", "", StringComparison.InvariantCulture)
                        .Replace("]", "", StringComparison.InvariantCulture);
                case Configuration.SqlEngine.SqLite:
                case Configuration.SqlEngine.PostgreSql:
                case Configuration.SqlEngine.OracleSql:
                    return identifier
                        .Replace("\"", "", StringComparison.InvariantCulture);
                case Configuration.SqlEngine.MySql:
                    return identifier
                        .Replace("`", "", StringComparison.InvariantCulture);
            }

            throw new NotSupportedException();
        }
    }
}