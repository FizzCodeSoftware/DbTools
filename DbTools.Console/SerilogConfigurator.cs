using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace FizzCode.DbTools.Console;
internal static class SerilogConfigurator
{
    public static ILogger CreateLogger(LogConfiguration? configuration)
    {
        var logsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log-dev");

        var config = new LoggerConfiguration()
            .Enrich.WithExceptionDetails()

            .WriteTo.File(new CompactJsonFormatter(), Path.Combine(logsFolder, "events-.json"),
                restrictedToMinimumLevel: configuration?.MinimumLogLevelInFile ?? LogEventLevel.Debug,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitInfo ?? int.MaxValue,
                rollingInterval: RollingInterval.Day,
                encoding: Encoding.UTF8)

            .WriteTo.File(Path.Combine(logsFolder, "2-info-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Information,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitInfo ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8)

            .WriteTo.File(Path.Combine(logsFolder, "3-warning-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Warning,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitImportant ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8)

            .WriteTo.File(Path.Combine(logsFolder, "4-error-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Error,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitImportant ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8)

            .WriteTo.File(Path.Combine(logsFolder, "5-fatal-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Fatal,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitImportant ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8);

        if ((configuration?.MinimumLogLevelInFile ?? LogEventLevel.Debug) <= LogEventLevel.Debug)
        {
            config.WriteTo.File(Path.Combine(logsFolder, "1-debug-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Debug,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitLow ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8);
        }

        if ((configuration?.MinimumLogLevelInFile ?? LogEventLevel.Debug) <= LogEventLevel.Verbose)
        {
            config.WriteTo.File(Path.Combine(logsFolder, "0-verbose-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Verbose,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitLow ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8);
        }

        var minimumLogLevelOnConsole = configuration?.MinimumLogLevelOnConsole ?? LogEventLevel.Debug;

        config.WriteTo.Sink(new ConsoleSink.ConsoleSink("{Timestamp:HH:mm:ss.fff} [{Level}] {Message} {Properties}{NewLine}{Exception}"), minimumLogLevelOnConsole);

        config = config.MinimumLevel.Is(System.Diagnostics.Debugger.IsAttached ? LogEventLevel.Verbose : minimumLogLevelOnConsole);

        if (System.Diagnostics.Debugger.IsAttached)
        {
            config = config.Enrich.WithThreadId();
        }

        if (configuration != null && !string.IsNullOrEmpty(configuration.SeqUrl) && configuration.SeqUrl != "-")
        {
            config = config.WriteTo.Seq(configuration.SeqUrl, apiKey: configuration.SeqApiKey);
        }

        return config.CreateLogger();
    }

    public static ILogger CreateOpsLogger(LogConfiguration? configuration)
    {
        var logsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log-ops");

        var loggerConfig = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(logsFolder, "2-info-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Information,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitInfo ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8)

            .WriteTo.File(Path.Combine(logsFolder, "3-warning-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Warning,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitImportant ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8)

            .WriteTo.File(Path.Combine(logsFolder, "4-error-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Error,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitImportant ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8)

            .WriteTo.File(Path.Combine(logsFolder, "5-fatal-.txt"),
                restrictedToMinimumLevel: LogEventLevel.Fatal,
                retainedFileCountLimit: configuration?.RetainedLogFileCountLimitImportant ?? int.MaxValue,
                outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:l} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture,
                encoding: Encoding.UTF8);

        loggerConfig = loggerConfig.MinimumLevel.Is(LogEventLevel.Information);

        return loggerConfig.CreateLogger();
    }
}