using System.Collections.Generic;
using LiveSearchEngine.Enums;
using LiveSearchEngine.Interfaces;

namespace ImGuiSniperHost.Common
{
    public class ImGuiLogger : ILogger
    {
        ImGuiLogger()
        {
        }

        public static readonly ImGuiLogger Instance = new ImGuiLogger();

        #region Implementation of ILogger

        public void Info(string message, params object[] args)
        {
            Log(LogLevel.Info, string.Format(message, args));
        }

        public void Warn(string message, params object[] args)
        {
            Log(LogLevel.Warn, string.Format(message, args));
        }

        public void Error(string message, params object[] args)
        {
            Log(LogLevel.Error, string.Format(message, args));
        }

        #endregion

        void Log(LogLevel level, string fmt)
        {
            _logs.Add((level, fmt));
        }

        public List<(LogLevel, string)> GetList() => _logs;
        public void Clear() => _logs.Clear();
        
        readonly List<(LogLevel, string)> _logs = new List<(LogLevel, string)>();
    }
}