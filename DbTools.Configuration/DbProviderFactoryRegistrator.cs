namespace FizzCode.DbTools.Configuration
{
    using System.Data.Common;
    using Microsoft.Extensions.Configuration;

    public static class DbProviderFactoryRegistrator
    {
        public static void LoadFromConfiguration(IConfigurationRoot configuration, string sectionKey = "DbProviderFactories")
        {
            var children = configuration
                .GetSection(sectionKey)
                .GetChildren();
            foreach (var child in children)
            {
                DbProviderFactories.RegisterFactory(child.GetValue<string>("InvariantName"), child.GetValue<string>("Type"));
            }
        }
    }
}
