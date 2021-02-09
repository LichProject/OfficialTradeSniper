using System.Collections.Generic;
using System.Linq;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;
using WebSocketSharp;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <inheritdoc/>
    public class OfficialTradeLiveSearch : BaseLiveSearchEngine<OfficialTradeConfiguration>
    {
        public OfficialTradeLiveSearch(ILogger logger, OfficialTradeConfiguration configuration)
            : base(logger, configuration)
        {
            _apiWrapper = new OfficialTradeApiWrapper(configuration);
        }

        ~OfficialTradeLiveSearch()
        {
            Close();
        }

        #region Implementation of IWebSocketConnectable

        /// <inheritdoc/>
        public override bool IsConnected => _webSockets.Any(x => x.ReadyState == WebSocketState.Open);

        /// <inheritdoc/>
        public override void Connect(SniperItem sniperItem)
        {
            var ws = InitializeWebSocket(sniperItem);
            ws.ConnectAsync();
        }

        /// <inheritdoc/>
        public override void Close()
        {
            foreach (var ws in _webSockets)
                ws.Close();
        }

        #endregion

        WebSocket InitializeWebSocket(SniperItem sniperItem)
        {
            var con = new OfficialTradeWsConnection(Logger, _apiWrapper) {OnItemFound = ValidationDelegate};
            var ws = con.Initialize(sniperItem, Configuration.PoeSessionId);

            _webSockets.Add(ws);

            return ws;
        }

        readonly List<WebSocket> _webSockets = new List<WebSocket>();
        readonly OfficialTradeApiWrapper _apiWrapper;
    }
}