using System;
using System.Linq;
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

                Configuration.LiveSearch.Run();
            }
            catch (Exception e)
            {
                Logger.Error("Не удалось запустить поиск.");
                Logger.Error(e.ToString());
            }
        }

        #endregion

        public StartLiveSearchCommand(CommandConfiguration config)
            : base(config)
        {
        }
    }
}