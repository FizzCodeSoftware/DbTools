namespace FizzCode.DbTools.Configuration
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public class ConnectionStringCollection
    {
        private readonly Dictionary<string, ConnectionStringWithProvider> _connectionStrings = new Dictionary<string, ConnectionStringWithProvider>();
        public IEnumerable<ConnectionStringWithProvider> All => _connectionStrings.Values;

        public void LoadFromConfiguration(IConfigurationRoot configuration, string path = "ConnectionStrings")
        {
            var connectionStrings = configuration
                .GetSection(path)
                .Get<ConnectionStringWithProvider[]>();

            if (connectionStrings == null)
                return;

            foreach (var connectionString in connectionStrings)
            {
                Add(connectionString);
            }
        }

        public void Add(ConnectionStringWithProvider connectionString)
        {
            _connectionStrings[connectionString.Name] = connectionString;
        }

        public ConnectionStringWithProvider this[string name]
        {
            get
            {
                _connectionStrings.TryGetValue(name, out var value);
                return value;
            }
        }
    }
}