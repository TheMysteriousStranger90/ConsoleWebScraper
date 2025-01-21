namespace ConsoleWebScraper.Logging;

public static class Logger
{
    private static string? _logPath;
    private static readonly object _lockObj = new object();

    public static void Initialize(string basePath)
    {
        _logPath = Path.Combine(basePath, "scraper.log");
    }

    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
        
        lock (_lockObj)
        {
            using (var fileStream = new FileStream(_logPath, 
                       FileMode.Append, 
                       FileAccess.Write, 
                       FileShare.ReadWrite))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(logMessage);
            }
        }

        var color = level switch
        {
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Success => ConsoleColor.Green,
            _ => ConsoleColor.Gray
        };

        Console.ForegroundColor = color;
        Console.WriteLine(logMessage.TrimEnd());
        Console.ResetColor();
    }
}