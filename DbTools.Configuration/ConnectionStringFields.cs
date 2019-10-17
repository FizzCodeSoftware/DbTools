namespace FizzCode.DbTools.Configuration
{
    public class ConnectionStringFields
    {
        public string Server { get; set; }
        public int? Port { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public bool? IntegratedSecurity { get; set; }
    }
}