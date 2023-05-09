namespace MatrixInversion.Library.Logging;

public static class Log
{
    public static ILog Default { get; } = new ConsoleLog();
}
