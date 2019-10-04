namespace FizzCode.DbTools.Common
{
    using Microsoft.Extensions.Configuration;

    public static class Configuration
    {
        public static IConfigurationRoot Load(string fileName)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"{fileName}.json", false)
                .AddJsonFile($"{fileName}-local.json", true)
                .Build();

            return configuration;
        }
    }
}
