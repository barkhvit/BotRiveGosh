using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace BotRiveGosh
{
    public static class Logger
    {
        private static ILogger _logger;

        public static void Initialize(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("BotRiveGosh");
        }

        public static void Log(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            var className = Path.GetFileNameWithoutExtension(filePath);
            var logMessage = $"{className}.{methodName}: {message}";

            _logger?.LogInformation(logMessage);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {logMessage}");
        }

        public static void LogError(string message, Exception ex = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            var className = Path.GetFileNameWithoutExtension(filePath);
            var logMessage = $"{className}.{methodName}: {message}";

            _logger?.LogError(ex, logMessage);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR: {logMessage}");
            if (ex != null)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
