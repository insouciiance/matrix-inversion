namespace MatrixInversion.Library.Logging;

public interface ILog
{
    void Write(string message, LogSeverity severity);
}
