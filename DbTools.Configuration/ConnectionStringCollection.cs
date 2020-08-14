namespace FizzCode.DbTools.Configuration
{
    using System.Collections.Generic;
    using FizzCode.LightWeight.Configuration;
    using Microsoft.Extensions.Configuration;

    public class ConnectionStringCollection
    {
        private readonly Dictionary<string, ConnectionStringWithProvider> _connectionStrings = new Dictionary<string, ConnectionStringWithProvider>();
        public IEnumerable<ConnectionStringWithProvider> All => _connectionStrings.Values;

        public void LoadFromConfiguration(IConfigurationRoot configuration, string sectionKey = "ConnectionStrings", IConfigurationSecretProtector secretProtector = null)
        {
            var children = configuration
                .GetSection(sectionKey)
                .GetChildren();

            foreach (var child in children)
            {
                var name = child.Key;
                var providerName = ConfigurationReader.GetCurrentValue(configuration, child.Path, "ProviderName", null, secretProtector);
                var connectionString = ConfigurationReader.GetCurrentValue(configuration, child.Path, "ConnectionString", null, secretProtector);
                var version = ConfigurationReader.GetCurrentValue(configuration, child.Path, "Version", null, secretProtector);

                Add(new ConnectionStringWithProvider(name, providerName, connectionString, version));
            }
        }

        public void Add(ConnectionStringWithProvider connectionString)
        {
            _connectionStrings[connectionString.Name.ToUpperInvariant()] = connectionString;
        }

        public IEnumerator<ConnectionStringWithProvider> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public ConnectionStringWithProvider this[string name]
        {
            get
            {
                name = name.ToUpperInvariant();
                _connectionStrings.TryGetValue(name, out var value);
                return value;
            }
        }
    }
}