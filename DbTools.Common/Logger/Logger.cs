namespace FizzCode.DbTools.Common.Logger
{
    using System;

    public class Logger
    {
        public EventHandler<LogEventArgs> LogEvent { get; set; }

        public void Log(LogSeverity severity, string text, params object[] args)
        {
            LogEvent?.Invoke(this, new LogEventArgs()
            {
                Text = text,
                Severity = severity,
                Arguments = args,
            });
        }

        public void LogOps(LogSeverity severity, string text, params object[] args)
        {
            LogEvent?.Invoke(this, new LogEventArgs()
            {
                Text = text,
                Severity = severity,
                Arguments = args,
                ForOps = true,
            });
        }
    }
}
