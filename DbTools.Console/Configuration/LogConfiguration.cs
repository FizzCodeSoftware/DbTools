using Serilog.Events;

namespace FizzCode.DbTools.Console;
public class LogConfiguration
{
    public string SeqUrl { get; set; }
    public string SeqApiKey { get; set; }
    public int RetainedLogFileCountLimitImportant { get; set; } = 30;
    public int RetainedLogFileCountLimitInfo { get; set; } = 14;
    public int RetainedLogFileCountLimitLow { get; set; } = 1;
    public LogEventLevel MinimumLogLevelOnConsole { get; set; }
    public LogEventLevel MinimumLogLevelInFile { get; set; }
}