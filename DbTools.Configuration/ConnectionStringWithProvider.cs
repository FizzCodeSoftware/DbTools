namespace FizzCode.DbTools.Configuration
{
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("{Name}, {ProviderName}, {ConnectionString}")]
    public class ConnectionStringWithProvider
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }

        public ConnectionStringWithProvider(string name, string providerName, string connectionString)
        {
            Name = name;
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Name, ProviderName, ConnectionString);
        }
    }
}