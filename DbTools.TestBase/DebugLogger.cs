using System;
using System.Collections.Generic;
using FizzCode.DbTools.Common.Logger;
using Serilog;
using Serilog.Events;

namespace FizzCode.DbTools.TestBase;
public class DebugLogger(ILogger logger)
{
    private readonly ILogger _logger = logger;

    public void OnLog(object? sender, LogEventArgs args)
    {
        if (args.Exception != null)
            OnException(sender, args);

        var values = new List<object?>();
        if(args.Arguments is not null)
            values.AddRange(args.Arguments);

        _logger.Write(
            (LogEventLevel)args.Severity,
            "[{Module}] " + args.Text,
            values.ToArray()
            );
    }

    private void OnException(object? sender, LogEventArgs args)
    {
        var opsErrors = new List<string>();
        GetOpsMessages(args.Exception!, opsErrors);
        foreach (var opsError in opsErrors)
        {
            OnLog(sender, new LogEventArgs()
            {
                Severity = LogSeverity.Fatal,
                Text = opsError,
                ForOps = true,
            });
        }

        var lvl = 0;
        var msg = "EXCEPTION: ";

        var ex = args.Exception;
        while (ex != null)
        {
            if (lvl > 0)
                msg += "\nINNER EXCEPTION: ";

            msg += ex.Message;

            ex = ex.InnerException;
            lvl++;
        }

        _logger.Fatal("[{Module}], {Message}", msg);
    }

    private void GetOpsMessages(Exception ex, List<string> messages)
    {
        if (ex.InnerException != null)
            GetOpsMessages(ex.InnerException, messages);

        if (ex is AggregateException aex)
        {
            foreach (var iex in aex.InnerExceptions)
            {
                GetOpsMessages(iex, messages);
            }
        }
    }
}
