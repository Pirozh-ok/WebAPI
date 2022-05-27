namespace Habr.Common.Logging
{
    public interface ILoggerManager
    {
        void LogInfo(string message);
        void LogWarning(string message);
    }
}
