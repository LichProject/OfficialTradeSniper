using System;
using TradeSniper.Interfaces;
using TradeSniper.Models;
using TradeSniper.Settings;

namespace TradeSniper.ConsoleCommands
{
    public class ShowSniperItemsCommand : BaseCommand
    {
        #region Implementation of IConsoleCommand

        public void Execute()
        {
            Console.Write("Название: ");
            string desc = Console.ReadLine();

            Console.Write("Хеш-код поиска: ");
            string hash = Console.ReadLine();

            Retry:
            Console.Write("Минимальный сток: ");
            string stringStock = Console.ReadLine();

            if (!int.TryParse(stringStock, out int stock) && stock >= 0)
                goto Retry;

            SniperSettings.AddSniper(new SniperItem(desc, hash, stock));
            SniperSettings.Save();

            Console.WriteLine("Добавлено, это уже {0} объект в списке.", SniperSettings.SniperItems.Count);
        }

        #endregion
    }
}