using System;
using LiveSearchEngine.Models;
using TradeSniper.Common.Utils;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public class AddSniperItemCommand : BaseConsoleCommand
    {
        #region Implementation of IConsoleCommand

        public override string Description => "Добавить предмет в снайпер.";
        public override string Alias => "add";

        public override void Execute()
        {
            Logger.Info("Название:");
            string desc = Console.ReadLine();

            Logger.Info("Хеш-код поиска:");
            string hash = Console.ReadLine();

            Logger.Info("Лига:");
            string league = Console.ReadLine();
            
            Configuration.SniperSettings.AddSniper(new SniperItem(desc, hash, league));
            Configuration.SniperSettings.Save();

            Logger.Info("Добавлено, это уже {0} объект в списке.", Configuration.SniperSettings.SniperItems.Count);
        }

        #endregion

        public AddSniperItemCommand(CommandConfiguration config)
            : base(config)
        {
        }
    }
}