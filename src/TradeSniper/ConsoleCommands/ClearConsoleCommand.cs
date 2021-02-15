using System;
using LiveSearchEngine.Models;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public class ClearConsoleCommand : BaseConsoleCommand
    {
        public ClearConsoleCommand(CommandConfiguration config)
            : base(config)
        {
        }

        #region Overrides of BaseConsoleCommand

        public override string Description => "Очистить консоль";
        public override string Alias => "clear";

        public override void Execute()
        {
            Console.Clear();
            Configuration.Menu.ShowMenu();
        }

        #endregion
    }
}