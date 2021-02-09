using System;
using System.Linq;
using LiveSearchEngine.Models;
using TradeSniper.Common.Utils;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public class RemoveSniperItemCommand : BaseConsoleCommand
    {
        #region Implementation of IConsoleCommand

        public override string Description => "Удалить предмет из снайп-листа";
        public override string Alias => "remove";

        public override void Execute()
        {
            Retry:
            Logger.Info("Номер в списке: ");
            if (!ConsoleUtils.GetIntegerFromLine(out int index))
                goto Retry;

            var item = Configuration.SniperSettings.SniperItems.ElementAtOrDefault(index);
            if (item == null)
                goto Retry;

            Configuration.SniperSettings.RemoveSniper(item);
            Configuration.SniperSettings.Save();

            Logger.Info("Объект {0} удален из списка.", item.Description);
        }

        #endregion

        public RemoveSniperItemCommand(CommandConfiguration config)
            : base(config)
        {
        }
    }
}