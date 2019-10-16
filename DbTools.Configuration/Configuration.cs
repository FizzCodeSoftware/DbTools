namespace FizzCode.DbTools.Common
{
    using Microsoft.Extensions.Configuration;

    public static class Configuration
    {
        public static IConfigurationRoot Load(string fileName, bool optional = false)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"{fileName}.json", optional)
                .AddJsonFile($"{fileName}-local.json", true)
                .Build();

            return configuration;
        }
    }
}
