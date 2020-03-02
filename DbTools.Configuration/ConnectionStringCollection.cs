namespace FizzCode.DbTools.Configuration
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public class ConnectionStringCollection
    {
        private readonly Dictionary<string, ConnectionStringWithProvider> _connectionStrings = new Dictionary<string, ConnectionStringWithProvider>();
        public IEnumerable<ConnectionStringWithProvider> All => _connectionStrings.Values;

        public void LoadFromConfiguration(IConfigurationRoot configuration, string sectionKey = "ConnectionStrings")
        {
            var children = configuration
                .GetSection(sectionKey)
                .GetChildren();

            foreach (var child in children)
            {
                Add(new ConnectionStringWithProvider(
                    name: child.Key,
                    providerName: child.GetValue<string>("ProviderName"),
                    connectionString: child.GetValue<string>("ConnectionString"),
                    version: child.GetValue<string>("Version")));
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