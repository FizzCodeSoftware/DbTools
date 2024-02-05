using System;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace FizzCode.DbTools;
public static class DbProviderFactoryRegistrator
{
    public static void LoadFromConfiguration(IConfigurationRoot configuration, string sectionKey = "DbProviderFactories")
    {
        var children = configuration
            .GetSection(sectionKey)
            .GetChildren();
        foreach (var child in children)
        {
            var invariantName = child.GetValue<string>("InvariantName");
            var type = child.GetValue<string>("Type");

            if (invariantName is null)
                throw new InvalidOperationException("InvariantName cannot be null in configuration.");

            if (type is null)
                throw new InvalidOperationException("Type cannot be null in configuration.");

            DbProviderFactories.RegisterFactory(invariantName, type);
        }
    }
}
