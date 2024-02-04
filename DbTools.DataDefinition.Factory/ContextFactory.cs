using FizzCode.DbTools.Common;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.LightWeight.Configuration;

namespace FizzCode.DbTools.DataDefinition.Factory;
public class ContextFactory : IContextFactory
{
    private readonly Logger _logger;
    public ContextFactory(Logger logger)
    {
        _logger = logger;
    }

    public Context CreateContext(SqlEngineVersion version)
    {
        var configuration = ConfigurationLoader.LoadFromJsonFile("config", true);
        var settings = Helper.GetDefaultSettings(version, configuration);
        var context = new Context
        {
            Settings = settings
        };
        return context;
    }

    public ContextWithLogger CreateContextWithLogger(SqlEngineVersion version)
    {
        var configuration = ConfigurationLoader.LoadFromJsonFile("config", true);
        var settings = Helper.GetDefaultSettings(version, configuration);
        var contextWithLogger = new ContextWithLogger
        {
            Settings = settings,
            Logger = _logger
        };
        return contextWithLogger;
    }
}