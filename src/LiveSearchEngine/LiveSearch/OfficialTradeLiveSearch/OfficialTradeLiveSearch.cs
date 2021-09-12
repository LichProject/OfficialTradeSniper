using System.Collections.Generic;
using LiveSearchEngine.Interfaces;
using Newtonsoft.Json;
using WebSocketSharp;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    public class OfficialTradeLiveSearch : LiveSearchEngineBase<OfficialTradeConfiguration>
    {
        const int MaxItemLimit = 10;
        
        bool _authenticated;
        
        public OfficialTradeLiveSearch(OfficialTradeConfiguration configuration)
            : base(configuration, new RateLimitWrapper())
        {
            ApiWrapper = new OfficialTradeApiWrapper(configuration, RateLimit);
        }

        public OfficialTradeApiWrapper ApiWrapper { get; }


        #region Overrides of BaseLiveSearchEngine<OfficialTradeConfiguration>

        public override void UseNewConfiguration(OfficialTradeConfiguration configuration)
        {
            base.UseNewConfiguration(configuration);
            ApiWrapper.UseNewConfiguration(configuration);
        }

        public override bool ValidateConfiguration()
        {
            return !string.IsNullOrEmpty(Configuration.PoeSessionId);
        }

        protected override WebSocket CreateWebSocket(ISniperItem sniperItem)
        {
            return new OfficialTradeWebSocketFactory().Create(sniperItem, Configuration.PoeSessionId);
        }

        #endregion

        #region WebSocket

        protected override void WsOnMessageReceived(ISniperItem sniperItem, MessageEventArgs e)
        {
            var response = e.Data;
            if (!_authenticated && response.Contains("auth"))
            {
                _authenticated = true;
                return;
            }

            var p = JsonConvert.DeserializeObject<Dictionary<string, Stack<string>>>(response);
            if (!p.ContainsKey("new"))
                return;
            
            var fetchResponse = ApiWrapper.Fetch(p["new"], sniperItem.SearchUrlWrapper.Hash, MaxItemLimit);
            if (fetchResponse == null)
                return;

            foreach (var result in fetchResponse.Result)
            {
                RaiseItemFoundEvent(sniperItem, result.Item, result.Listing);
            }
        }

        #endregion
    }
}