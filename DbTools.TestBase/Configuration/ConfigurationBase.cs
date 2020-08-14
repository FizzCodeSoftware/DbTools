namespace FizzCode.DbTools.TestBase
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.LightWeight.Configuration;
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
            Configuration = ConfigurationLoader.LoadFromJsonFile(ConfigurationFileName);
            ConnectionStrings.LoadFromConfiguration(Configuration);
        }

        public virtual string ConfigurationFileName { get; }

        public IConfigurationRoot Configuration { get; private set; }

        public ConnectionStringCollection ConnectionStrings { get; } = new ConnectionStringCollection();
    }
}