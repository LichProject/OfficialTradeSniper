using TradeSniper.Common.Delegates;
using TradeSniper.Enums;

namespace TradeSniper.Common
{
    public sealed class MiniLogger
    {
        public event LogMessageDelegate OnLogMessage;

        public void Log(LogLevel level, string message)
        {
            OnLogMessage?.Invoke(level, message);
        }
    }
}