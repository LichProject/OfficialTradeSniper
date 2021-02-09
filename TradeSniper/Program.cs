using System;
using System.Diagnostics;
using LiveSearchEngine.Enums;
using LiveSearchEngine.LiveSearch;
using LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch;
using LiveSearchEngine.Models;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;
using TradeSniper.Common;
using TradeSniper.ConsoleCommands;
using TradeSniper.Models;
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

            var lsConfiguration = new OfficialTradeConfiguration
            {
                PoeSessionId = GlobalSettings.PoeSessionId,
                DelayFactor = GlobalSettings.RequestsDelayFactor
            };

            var lsEngine = new OfficialTradeLiveSearch(logger, lsConfiguration);

            var ls = new LiveSearchWrapper(lsEngine);
            ls.SetSniperList(SniperSettings.SniperItems);
            ls.Subscribe(OnItemFound);

            var config = new CommandConfiguration
            {
                Menu = conMenu,
                GlobalSettings = GlobalSettings,
                SniperSettings = SniperSettings,
                LiveSearch = ls
            };

            conMenu.UseCommandConfiguration(config);

            conMenu.AddCommand<StartLiveSearchCommand>();
            conMenu.AddCommand<StopLiveSearchCommand>();
            conMenu.AddCommand<ShowMenuCommand>();
            conMenu.AddCommand<ShowSniperItemsCommand>();
            conMenu.AddCommand<AddSniperItemCommand>();
            conMenu.AddCommand<RemoveSniperItemCommand>();
            conMenu.AddCommand<GlobalSettingsEditorCommand>();
            conMenu.AddCommand<ResendLatestTradeMessageCommand>();
            conMenu.AddCommand<ClearConsoleCommand>();

            conMenu.Run();
        }

        static void OnItemFound(SniperItem sniperItem, Item item, Listing listing)
        {
            if (listing.Price == null)
                return;

            var sw = Stopwatch.StartNew();

            var sellerName = listing.Account.LastCharacterName;
            var gameWhisperFmt = Game.FmtGameMessage(item, listing);

            Console.WriteLine($"[New item from {sellerName}] {gameWhisperFmt}");

            Game.SendChat(gameWhisperFmt);

            sw.Stop();
            Console.WriteLine("Сообщение отправлено спустя: " + sw.ElapsedMilliseconds + " мс");
        }

        static void OnLogMessage(LogLevel level, string message) => Console.WriteLine("[{0}] {1}", level, message);
    }
}