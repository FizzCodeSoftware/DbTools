using System;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.Factory.Interfaces;

namespace FizzCode.DbTools.TestBase;
public class TestContextFactory(Action<Settings>? settingsSetter) : IContextFactory
{
    private readonly Action<Settings>? _settingsSetter = settingsSetter;

    public Context CreateContext(SqlEngineVersion version)
    {
        var settings = TestHelper.GetDefaultTestSettings(version);
        _settingsSetter?.Invoke(settings);
        var context = new Context
        {
            Settings = settings
        };
        return context;
    }

    public ContextWithLogger CreateContextWithLogger(SqlEngineVersion version)
    {
        var settings = TestHelper.GetDefaultTestSettings(version);
        _settingsSetter?.Invoke(settings);
        var contextWithLogger = new ContextWithLogger
        {
            Settings = settings,
            Logger = TestHelper.CreateLogger()
        };
        return contextWithLogger;
    }
}
