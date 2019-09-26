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

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Name, ProviderName, ConnectionString);
        }
    }
}