using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace FizzCode.DbTools.TestBase;
internal static class SerilogConfigurator
{
    public static ILogger CreateLogger(LogConfiguration? configuration)
    {
        var minimumLevel = configuration?.MinimumLogLevelOnDebug ?? LogEventLevel.Verbose;

        var config = new LoggerConfiguration()
            .Enrich.WithExceptionDetails()
            .WriteTo.Debug(minimumLevel, "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message} {Properties}{NewLine}{Exception}");

        config = config.MinimumLevel.Is(minimumLevel);

        if (System.Diagnostics.Debugger.IsAttached)
        {
            config = config.Enrich.WithThreadId();
        }

        return config.CreateLogger();
    }
}