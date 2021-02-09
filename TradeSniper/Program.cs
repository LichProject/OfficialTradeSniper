using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using LiveSearchEngine.Enums;
using LiveSearchEngine.Models;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;
using TradeSniper.Common;
using TradeSniper.ConsoleCommands;
using TradeSniper.Models;
using TradeSniper.Native;
using TradeSniper.Settings;

namespace TradeSniper
{
    class Program
    {
        static readonly GlobalSettings GlobalSettings = new GlobalSettings();
        static readonly SniperSettings SniperSettings = new SniperSettings();

        static void Main(string[] args)
        {
            var logger = new Logger();
            logger.OnLogMessage += OnLogMessage;

            var conMenu = new ConsoleMenu(logger);
            var config = new CommandConfiguration
            {
                Menu = conMenu,
                GlobalSettings = GlobalSettings,
                SniperSettings = SniperSettings
            };

            conMenu.UseCommandConfiguration(config);

            conMenu.AddCommand(new StartLiveSearchCommand(config, OnItemFound));
            conMenu.AddCommand<StopLiveSearchCommand>();
            conMenu.AddCommand<ShowMenuCommand>();
            conMenu.AddCommand<ShowSniperItemsCommand>();
            conMenu.AddCommand<AddSniperItemCommand>();
            conMenu.AddCommand<RemoveSniperItemCommand>();
            conMenu.AddCommand<GlobalSettingsEditorCommand>();
            //menu.AddCommand<SniperSettingsEditorCommand>();
            conMenu.AddCommand<ClearConsoleCommand>();

            conMenu.Run();
        }

        static void OnItemFound(SniperItem sniperItem, Item item, Listing listing)
        {
            if (listing.Price == null)
                return;

            if (!item.StackSize.HasValue || !sniperItem.CompareStock(item.StackSize.Value))
                return;

            var sw = Stopwatch.StartNew();

            var sellerName = listing.Account.LastCharacterName;
            var itemName = $"{item.Name} {item.BaseType}".TrimStart();
            var itemStock = item.StackSize ?? 1;
            var league = item.League;
            var price = listing.Price;
            var priceAmount = price.Amount * itemStock;
            var currency = price.Currency;

            if (currency.Equals("chaos"))
                currency = "Chaos Orb";
            else
                return;

            var itemFmt = $"{itemStock} {itemName} for my {priceAmount} {currency} in {league}.";
            var gameWhisperFmt = $"@{sellerName} Hi, I'd like to buy your {itemFmt}";

            Console.WriteLine($"[New item from {sellerName}] {itemFmt}");

            Interop.SetForegroundWindow(Game.Proc.MainWindowHandle);
            Interop.SetActiveWindow(Game.Proc.MainWindowHandle);
            
            Input.InjectKey(Keys.Enter);
            Input.SimulateTextEntry(gameWhisperFmt);
            Thread.Sleep(40);
            Input.InjectKey(Keys.Enter);

            sw.Stop();
            Console.WriteLine("Сообщение отправлено спустя: " + sw.ElapsedMilliseconds + " мс");
        }

        static void OnLogMessage(LogLevel level, string message) => Console.WriteLine("[{0}] {1}", level, message);
    }
}