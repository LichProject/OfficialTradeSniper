using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public class ShowSniperItemsCommand : BaseConsoleCommand
    {
        #region Implementation of IConsoleCommand

        public override string Description => "Показать снайпер-лист";
        public override string Alias => "list";

        public override void Execute()
        {
            var items = Configuration.SniperSettings.SniperItems;
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                Logger.Info("[{0}] {1} -> {2}", i, item.Description, item.SearchHash);
            }
        }

        #endregion

        public ShowSniperItemsCommand(CommandConfiguration config)
            : base(config)
        {
        }
    }
}