using Google.Cloud.Spanner.V1.Internal.Logging;
using LogLevel = Google.Cloud.Spanner.V1.Internal.Logging.LogLevel;

namespace SpannerWebApi;

public class ConsoleLogger : Logger
{
    public static ConsoleLogger Instance { get; } = new ConsoleLogger();

    private ConsoleLogger() => LogLevel = LogLevel.Debug;
    
    public static void Install() => SetDefaultLogger(Instance);

    protected override void LogImpl(LogLevel level, string message, Exception exception) =>
        WriteLine(exception == null ? $"{level}: {message}" : $"{level}: {message}, Exception: {exception}");

    protected override void LogPerformanceEntries(IEnumerable<string> entries)
    {
        WriteLine("Performance:");
        foreach (var entry in entries)
        {
            WriteLine($"  {entry}");
        }
    }

    private static void WriteLine(string line) => Console.WriteLine(line);
}