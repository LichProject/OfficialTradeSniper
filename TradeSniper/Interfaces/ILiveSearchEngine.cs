using TradeSniper.Delegates;
using TradeSniper.Models;

namespace TradeSniper.Interfaces
{
    public interface IWebSocketConnectable
    {
        event ItemFoundDelegate OnItemFound;
        event LogMessageDelegate OnLogMessage;

        bool IsConnected { get; }

        void Connect(LiveUrlWrapper liveUrlWrapper);
        void Close();
    }
}