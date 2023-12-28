namespace AElf.Contract.SourceGenerator.Logging;

public interface ILogger
{
    bool IsEnabled(LogLevel logLevel);

    void Log(LogLevel logLevel, string message);
}