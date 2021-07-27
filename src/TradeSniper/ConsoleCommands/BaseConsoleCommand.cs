using System;
using LiveSearchEngine.Interfaces;
using TradeSniper.Interfaces;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public abstract class BaseConsoleCommand : IConsoleCommand
    {
        protected BaseConsoleCommand(CommandConfiguration config)
        {
            Configuration = config;
        }

        protected CommandConfiguration Configuration { get; }
        protected ILogger Logger => Configuration.Menu.Logger;

        #region Implementation of IConsoleCommand

        public abstract string Description { get; }
        public abstract string Alias { get; }
        public virtual Func<bool> ExecuteCondition => null;
        public abstract void Execute();

        #endregion
    }
}