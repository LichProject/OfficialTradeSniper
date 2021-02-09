using System;
using System.Linq;
using TradeSniper.Interfaces;
using TradeSniper.Settings;

namespace TradeSniper.ConsoleCommands
{
    public class RemoveSniperItemCommand : BaseCommand
    {
        #region Implementation of IConsoleCommand

        public override void Execute()
        {
            Retry:
            Console.Write("Номер в списке: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out int index))
                goto Retry;

            var item = SniperSettings.SniperItems.ElementAtOrDefault(index);
            if (item == null)
                goto Retry;

            SniperSettings.RemoveSniper(item);
            SniperSettings.Save();

            Console.WriteLine("Объект {0} удален из списка.", item.Description);
        }

        #endregion

        public RemoveSniperItemCommand(ILogger logger, GlobalSettings globalSettings, SniperSettings sniperSettings)
            : base(logger, globalSettings, sniperSettings)
        {
        }
    }
}