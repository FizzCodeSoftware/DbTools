namespace FizzCode.DbTools.Configuration
{
    public class ConnectionStringWithProvider
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }
    }
}