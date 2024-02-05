namespace FizzCode.DbTools.Common;

public class ContextWithLogger : Context
{
    public required Logger.Logger Logger { get; init; }
}
