namespace Game.Core.Abstractions
{
    /// <summary>
    /// Interface for logging. to abstract logging from core logic.
    /// </summary>
    public interface ILogger
    {
        void Log(object message, object context = null);
        void LogError(object message, object context = null);
        void LogWarning(string message, object context = null);
        void LogException(System.Exception exception, object context = null);
    }
}