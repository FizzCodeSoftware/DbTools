namespace FizzCode.DbTools.Configuration
{
    using System.Collections.Generic;
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
                var providerName = child.GetValue<string>("ProviderName");
                var connectionString = child.GetValue<string>("ConnectionString");
                var version = child.GetValue<string>("Version");

                if (secretProtector != null)
                {
                    if (child.GetValue("ProviderName-protected", false))
                        providerName = secretProtector.Decrypt(providerName);

                    if (child.GetValue("ConnectionString-protected", false))
                        connectionString = secretProtector.Decrypt(connectionString);

                    if (child.GetValue("Version-protected", false))
                        version = secretProtector.Decrypt(version);
                }

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