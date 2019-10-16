namespace FizzCode.DbTools.TestBase
{
    using FizzCode.DbTools.Configuration;
    using Microsoft.Extensions.Configuration;

    public class ConfigurationBase
    {
        public ConfigurationBase()
        {
            Initialize();
        }

        public ConfigurationBase(string configurationFileName)
        {
            ConfigurationFileName = configurationFileName;
            Initialize();
        }

        private void Initialize()
        {
            Configuration = Common.Configuration.Load(ConfigurationFileName);
            ConnectionStrings.LoadFromConfiguration(Configuration);
        }

        public virtual string ConfigurationFileName { get; }

        public IConfigurationRoot Configuration { get; private set; }

        public ConnectionStringCollection ConnectionStrings { get; } = new ConnectionStringCollection();
    }
}