using LiveSearchEngine.Delegates;
using LiveSearchEngine.Enums;
using LiveSearchEngine.Interfaces;

namespace TradeSniper.Common
{
    public sealed class Logger : ILogger
    {
        public event LogMessageDelegate OnLogMessage;

        void Log(LogLevel level, string message)
        {
            OnLogMessage?.Invoke(level, message);
        }

        public void Info(string message, params object[] args)
        {
            Log(LogLevel.Info, Fmt(message, args));
        }

        public void Warn(string message, params object[] args)
        {
            Log(LogLevel.Warn, Fmt(message, args));
        }

        public void Error(string message, params object[] args)
        {
            Log(LogLevel.Error, Fmt(message, args));
        }

        string Fmt(string message, params object[] args) => string.Format(message, args);
    }
}