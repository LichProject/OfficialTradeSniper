using System;
using System.Linq;
using LiveSearchEngine.Delegates;
using LiveSearchEngine.LiveSearch;
using LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public class StartLiveSearchCommand : BaseConsoleCommand
    {
        #region Overrides of BaseConsoleCommand

        public override string Description => "Запустить";
        public override string Alias => "start";
        public override Func<bool> ExecuteCondition => () => Configuration.LiveSearch == null || !Configuration.LiveSearch.IsRunning;

        public override void Execute()
        {
            try
            {
                if (Configuration.GlobalSettings.PoeSessionId == null)
                {
                    Logger.Warn("Не указан POESESSID");
                    return;
                }

                if (!Configuration.SniperSettings.SniperItems.Any())
                {
                    Logger.Warn("Не заполнен снайпер-лист.");
                    return;
                }
                
                var lsConfiguration = new OfficialTradeConfiguration
                {
                    PoeSessionId = Configuration.GlobalSettings.PoeSessionId,
                    DelayFactor = Configuration.GlobalSettings.RequestsDelayFactor
                };

                var lsEngine = new OfficialTradeLiveSearch(Logger, lsConfiguration);
                
                Configuration.LiveSearch = new LiveSearchWrapper(lsEngine);
                Configuration.LiveSearch.SetSniperList(Configuration.SniperSettings.SniperItems);
                Configuration.LiveSearch.Subscribe(_itemFoundDelegate);
                
                Configuration.LiveSearch.Run();
            }
            catch (Exception e)
            {
                Logger.Error("Не удалось запустить поиск.");
                Logger.Error(e.ToString());
            }
        }

        #endregion

        public StartLiveSearchCommand(CommandConfiguration config, ItemFoundDelegate itemFoundDelegate)
            : base(config)
        {
            _itemFoundDelegate = itemFoundDelegate;
        }

        readonly ItemFoundDelegate _itemFoundDelegate;
    }
}