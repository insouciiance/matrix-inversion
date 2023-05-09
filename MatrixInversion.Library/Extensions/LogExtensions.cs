using System;
using MatrixInversion.Library.Logging;

namespace MatrixInversion.Library.Extensions;

public static class LogExtensions
{
    public static void WriteLine(this ILog log, string message, LogSeverity severity)
    {
        log.Write(message + Environment.NewLine, severity);
    }

    public static void Info(this ILog log, string message)
    {
        log.WriteLine(message, LogSeverity.Info);
    }

    public static void Warning(this ILog log, string message)
    {
        log.WriteLine(message, LogSeverity.Warning);
    }

    public static void Error(this ILog log, string message)
    {
        log.WriteLine(message, LogSeverity.Error);
    }
}
