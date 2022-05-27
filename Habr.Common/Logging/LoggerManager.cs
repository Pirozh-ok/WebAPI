using NLog;

namespace Habr.Common.Logging
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        public void LogInfo(string message) => _logger.Info(message);
        public void LogWarning(string message) => _logger.Warn(message);
        public void LogError(string message) => _logger.Error(message);
    }
}
