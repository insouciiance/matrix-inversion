using System;

namespace MatrixInversion.Library.Logging;

public class ConsoleLog : ILog
{
    public void Write(string message, LogSeverity severity)
    {
        Console.Write(GetPrefix(severity) + message);
    }

    private static string GetPrefix(LogSeverity severity) => severity switch
    {
        LogSeverity.Info => "",
        LogSeverity.Warning => "WARNING: ",
        LogSeverity.Error => "ERROR: ",
        _ => throw new IndexOutOfRangeException()
    };
}
