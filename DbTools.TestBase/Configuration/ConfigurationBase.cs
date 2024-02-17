using System.Diagnostics.CodeAnalysis;
using FizzCode.LightWeight.AdoNet;
using FizzCode.LightWeight.Configuration;
using Microsoft.Extensions.Configuration;

namespace FizzCode.DbTools.TestBase;
public class ConfigurationBase
{
    
    /*public ConfigurationBase()
    {
        Initialize();
    }*/

    public ConfigurationBase(string configurationFileName)
    {
        ConfigurationFileName = configurationFileName;
        Initialize();
    }

    //[MemberNotNull(nameof(ConfigurationFileName))]
    [MemberNotNull(nameof(Configuration))]
    private void Initialize()
    {
        Configuration = ConfigurationLoader.LoadFromJsonFile(ConfigurationFileName);
        ConnectionStrings.LoadFromConfiguration(Configuration);
    }

    public string ConfigurationFileName { get; }

    public IConfigurationRoot Configuration { get; private set; }

    public ConnectionStringCollection ConnectionStrings { get; } = new ConnectionStringCollection();
}