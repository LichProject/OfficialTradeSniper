using System;
using ImGuiNET;
using ImGuiSniperHost.Common;
using ImGuiSniperHost.Settings;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.LiveSearch;
using LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;

namespace ImGuiSniperHost.Controllers
{
    public class SniperController : IDrawController
    {
        public SniperController()
        {
            ReloadLiveSearchEngine();
        }

        #region Overrides of GuiControllerBase

        public void Draw()
        {
            if (_liveSearch.IsRunning)
            {
                if (ImGui.Button("Stop"))
                {
                    Logger.Info("Now requesting the engine to stop.");

                    _liveSearch.Stop();
                    return;
                }
            }
            else if (ImGui.Button("Start"))
            {
                if (!_liveSearch.Run())
                {
                    Logger.Error("Cannot run the livesearch at the moment.");
                    return;
                }

                return;
            }

            ImGui.SameLine();

            if (ImGui.Button("Reload Engine"))
            {
                if (_liveSearch.IsRunning)
                {
                    Logger.Warn("You cannot reload when the engine is running.");
                    return;
                }
                
                ReloadLiveSearchEngine();
                Logger.Warn("Livesearch configuration has been updated.");
            }
        }

        #endregion

        void ReloadLiveSearchEngine()
        {
            var cfg = new OfficialTradeConfiguration
            {
                PoeSessionId = GlobalSettings.PoeSessionId,
                DelayFactor = GlobalSettings.RequestsDelayFactor
            };

            if (_engine == null || _liveSearch == null)
            {
                _engine = new OfficialTradeLiveSearch(Logger, cfg);
                _engine.Disconnected += OnWsDisconnected;
                _engine.Error += OnWsError;

                _liveSearch = new LiveSearchWrapper(_engine);
                _liveSearch.Subscribe(OnItemFound);
                _liveSearch.OnStop += LiveSearchOnStop;
            }
            else
            {
                _engine.UseNewConfiguration(cfg);
            }

            _liveSearch.SetSniperList(SniperSettings.SniperItems);
        }

        void OnWsError(ISniperItem sniperitem, Exception exception)
        {
            Logger.Warn("Sniper (Description: {0}) thrown an exception:\n{1}", sniperitem.Description, exception);
        }

        void OnWsDisconnected(ISniperItem sniperitem, Exception exception)
        {
            Logger.Warn("Connection to {0} has been closed.", sniperitem.SearchUrlWrapper.SearchUrl);
        }

        void LiveSearchOnStop(object sender, EventArgs e)
        {
            Logger.Info("Livesearch engine has been stopped.");
        }

        void OnItemFound(ISniperItem sniperItem, Item item, Listing listing)
        {
            // TODO: TEMPORARY LOG.
            // TODO: Let this method to throw control to the other classes.

            var name = $"{item.Name} {item.BaseType}".TrimStart();
            var price = listing.Price == null
                ? "NO PRICE"
                : $"{listing.Price.Amount}x {listing.Price.Currency}";

            Logger.Info(
                "[New item from {0}] Sniper description: {3} | Name: {1} | Price: {2}",
                listing.Account.LastCharacterName,
                name,
                price,
                sniperItem.Description);
        }

        GlobalSettings GlobalSettings => GlobalSettings.Instance;
        SniperSettings SniperSettings => SniperSettings.Instance;

        static ImGuiLogger Logger => ImGuiLogger.Instance;

        OfficialTradeLiveSearch _engine;
        LiveSearchWrapper _liveSearch;
    }
}