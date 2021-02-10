using ImGuiNET;
using ImGuiSniperHost.Common;
using ImGuiSniperHost.Settings;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.LiveSearch;
using LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch;
using LiveSearchEngine.Models;
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
                    _liveSearch.Stop();
                    return;
                }
            }
            else if (ImGui.Button("Start"))
            {
                if (!_liveSearch.Run())
                {
                    _logger.Error("Cannot start the livesearch (sniper-list not specified?)");
                    return;
                }

                return;
            }

            ImGui.SameLine();

            if (ImGui.Button("Reload Engine"))
            {
                ReloadLiveSearchEngine();
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
                _engine = new OfficialTradeLiveSearch(_logger, cfg);
                _liveSearch = new LiveSearchWrapper(_engine);
                _liveSearch.Subscribe(OnItemFound);
            }
            else
            {
                _engine.UseNewConfiguration(cfg);
            }

            _liveSearch.SetSniperList(SniperSettings.SniperItems);
        }

        void OnItemFound(SniperItem sniperItem, Item item, Listing listing)
        {
            // throw control to the other classes (like validators / message logic)
        }

        GlobalSettings GlobalSettings => GlobalSettings.Instance;
        SniperSettings SniperSettings => SniperSettings.Instance;

        readonly ILogger _logger = new ImGuiLogger();

        OfficialTradeLiveSearch _engine;
        LiveSearchWrapper _liveSearch;
    }
}