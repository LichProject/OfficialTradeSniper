using LiveSearchEngine.Models;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public class ShowMenuCommand : BaseConsoleCommand
    {
        #region Overrides of BaseConsoleCommand

        public override string Description => "Показать список меню.";
        public override string Alias => "menu";

        public override void Execute()
        {
            Configuration.Menu.ShowMenu();
        }

        #endregion

        public ShowMenuCommand(CommandConfiguration config)
            : base(config)
        {
        }
    }
}