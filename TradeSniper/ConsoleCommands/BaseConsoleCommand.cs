using TradeSniper.Interfaces;
using TradeSniper.Settings;

namespace TradeSniper.ConsoleCommands
{
    public abstract class BaseConsoleCommand : IConsoleCommand
    {
        protected BaseConsoleCommand(ILogger logger, GlobalSettings globalSettings, SniperSettings sniperSettings)
        {
            GlobalSettings = globalSettings;
            SniperSettings = sniperSettings;
            Logger = logger;
        }

        public ILogger Logger { get; }
        protected GlobalSettings GlobalSettings { get; }
        protected SniperSettings SniperSettings { get; }

        #region Implementation of IConsoleCommand

        public abstract void Execute();

        #endregion
    }
}