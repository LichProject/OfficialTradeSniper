using TradeSniper.Interfaces;
using TradeSniper.Models;
using TradeSniper.Settings;

namespace TradeSniper.Common
{
    public sealed class LiveSearchWrapper
    {
        public LiveSearchWrapper(ILogger logger, ILiveSearchEngine engine, GlobalSettings globalSettings, SniperSettings sniperSettings)
        {
            Engine = engine;
            _logger = logger;
            _globalSettings = globalSettings;
            _sniperSettings = sniperSettings;
        }

        public bool IsRunning => Engine.IsConnected;
        public ILiveSearchEngine Engine { get; }

        public void Run()
        {
            foreach (var si in _sniperSettings.SniperItems)
            {
                var url = new LiveUrlWrapper(si.SearchHash, _globalSettings.League);
                Engine.Connect(url);

                _logger.Info($"[LiveSearch::Run] <{si.Description}> {url.SearchUrl}");
            }
        }

        public void Stop()
        {
            Engine.Close();
        }

        readonly ILogger _logger;
        readonly GlobalSettings _globalSettings;
        readonly SniperSettings _sniperSettings;
    }
}