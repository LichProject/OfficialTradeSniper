using System.Collections.Generic;
using System.Linq;
using LiveSearchEngine.Delegates;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;
using LiveSearchEngine.Models.Default;
using WebSocket4Net;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <inheritdoc/>
    public class OfficialTradeLiveSearch : BaseLiveSearchEngine<OfficialTradeConfiguration>
    {
        public OfficialTradeLiveSearch(ILogger logger, OfficialTradeConfiguration configuration)
            : base(logger, configuration)
        {
            ApiWrapper = new OfficialTradeApiWrapper(configuration);
        }

        ~OfficialTradeLiveSearch()
        {
            Close();
        }

        public WebSocketDisconnectedDelegate Disconnected;
        public WebSocketDisconnectedDelegate Error;

        public OfficialTradeApiWrapper ApiWrapper { get; }

        #region Implementation of IWebSocketConnectable

        #region Overrides of BaseLiveSearchEngine<OfficialTradeConfiguration>

        public override void UseNewConfiguration(OfficialTradeConfiguration configuration)
        {
            base.UseNewConfiguration(configuration);
            ApiWrapper.UseNewConfiguration(configuration);
        }

        #endregion

        /// <inheritdoc/>
        public override bool IsConnected => _webSockets.Any(x => x.State == WebSocketState.Open);

        /// <inheritdoc/>
        public override void Connect(ISniperItem sniperItem)
        {
            var ws = CreateWebSocketClient(sniperItem);
            _webSockets.Add(ws);
            ws.Open();
        }

        /// <inheritdoc/>
        public override void Close()
        {
            foreach (var ws in _webSockets)
                ws.Close();
        }

        #endregion

        OfficialTradeWebSocketFactory GetFactory()
        {
            return new OfficialTradeWebSocketFactory(ApiWrapper)
            {
                ItemFound = ValidationDelegate,
                Disconnected = Disconnected,
                Error = Error
            };
        }

        WebSocket CreateWebSocketClient(ISniperItem item)
        {
            return GetFactory().Create(item, Configuration.PoeSessionId);
        }

        readonly List<WebSocket> _webSockets = new List<WebSocket>();
    }
}