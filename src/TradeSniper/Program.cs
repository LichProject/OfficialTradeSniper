﻿using System;
using LiveSearchEngine.Enums;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.LiveSearch;
using LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch;
using LiveSearchEngine.Models.Default;
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
            lsEngine.Disconnected += OnWsDisconnected;
            lsEngine.Error += OnWsDisconnected;

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

        static void OnWsDisconnected(ISniperItem sniperitem, Exception e)
        {
            bool hasError = e != null;

            Console.WriteLine($"{sniperitem.SearchUrlWrapper.SearchUrl} - connection {(hasError ? "has an error" : "was closed")}.");
            if (hasError)
                Console.WriteLine(e.ToString());
        }

        static void OnItemFound(ISniperItem sniperItem, Item item, Listing listing)
        {
            if (listing.Price == null)
                return;

#if !DEBUG
            var sw = Stopwatch.StartNew();
#endif
            var sellerName = listing.Account.LastCharacterName;
            var gameWhisperFmt = Game.FmtGameMessage(item, listing);

            Console.WriteLine($"[New item from {sellerName}] {gameWhisperFmt}");

#if !DEBUG
            Game.SendChat(gameWhisperFmt);

            sw.Stop();
            Console.WriteLine("Сообщение отправлено спустя: " + sw.ElapsedMilliseconds + " мс");
#endif
        }

        static void OnLogMessage(LogLevel level, string message) => Console.WriteLine("[{0}] {1}", level, message);
    }
}