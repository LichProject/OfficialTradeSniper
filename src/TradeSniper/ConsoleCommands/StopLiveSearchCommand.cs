using System;
using LiveSearchEngine.Models;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public class StopLiveSearchCommand : BaseConsoleCommand
    {
        #region Overrides of BaseConsoleCommand

        public override string Description => "Остановить";
        public override string Alias => "stop";

        public override Func<bool> ExecuteCondition => () => Configuration.LiveSearch != null && Configuration.LiveSearch.IsRunning;

        public override void Execute()
        {
            Configuration.LiveSearch.Stop();
        }

        #endregion

        public StopLiveSearchCommand(CommandConfiguration config)
            : base(config)
        {
        }
    }
}