using System;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;
using LiveSearchEngine.Models.Default;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;
using TradeSniper.Common;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public class ResendLatestTradeMessageCommand : BaseConsoleCommand
    {
        #region Overrides of BaseConsoleCommand

        public override string Description => "Отправить последнее сообщение в игровой чат.";

        public override string Alias => "resend";

        public override Func<bool> ExecuteCondition => () => Configuration.LiveSearch != null && Configuration.LiveSearch.IsRunning;

        public override void Execute()
        {
            if (_latestGameMessage != null)
                Game.SendChat(_latestGameMessage);
        }

        #endregion

        public ResendLatestTradeMessageCommand(CommandConfiguration config)
            : base(config)
        {
            Configuration.LiveSearch.Subscribe(OnItemFound);
        }

        void OnItemFound(ISniperItem sniperitem, Item item, Listing listing)
        {
            if (listing.Price == null)
                return;

            _latestGameMessage = Game.FmtGameMessage(item, listing);
        }

        string _latestGameMessage;
    }
}