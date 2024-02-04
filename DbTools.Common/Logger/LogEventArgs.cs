using System;

namespace FizzCode.DbTools.Common.Logger;
public enum LogSeverity
{
    Verbose = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Fatal = 5,
}

public class LogEventArgs : EventArgs
{
    public string Text { get; set; }
    public object[] Arguments { get; set; }
    public LogSeverity Severity { get; set; }
    public bool ForOps { get; set; }
    public Exception Exception { get; set; }
}