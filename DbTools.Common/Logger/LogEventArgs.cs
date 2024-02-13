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
    public required string Text { get; init; }
    public object?[]? Arguments { get; init; }
    public LogSeverity Severity { get; init; }
    public bool ForOps { get; init; }
    public Exception? Exception { get; init; }
}