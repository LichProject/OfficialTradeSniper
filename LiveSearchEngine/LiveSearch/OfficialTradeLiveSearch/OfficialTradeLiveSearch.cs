using System.Collections.Generic;
using System.Linq;
using LiveSearchEngine.Delegates;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;
using WebSocketSharp;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <inheritdoc/>
    public class OfficialTradeLiveSearch : ILiveSearchEngine
    {
        /// <inheritdoc/>
        public event ItemFoundDelegate OnItemFound;

        public OfficialTradeLiveSearch(ILogger logger, OfficialTradeConfiguration configuration)
        {
            Logger = logger;
            _configuration = configuration;
            _apiWrapper = new OfficialTradeApiWrapper(configuration);
        }

        ~OfficialTradeLiveSearch()
        {
            Close();
        }

        #region Implementation of IWebSocketConnectable

        /// <inheritdoc/>
        public ILogger Logger { get; }
        
        /// <inheritdoc/>
        public bool IsConnected => _webSockets.Any(x => x.ReadyState == WebSocketState.Open);

        /// <inheritdoc/>
        public void Connect(SniperItem sniperItem)
        {
            var ws = InitializeWebSocket(sniperItem);
            ws.ConnectAsync();
        }

        /// <inheritdoc/>
        public void Close()
        {
            foreach (var ws in _webSockets)
                ws.Close();
        }

        #endregion

        WebSocket InitializeWebSocket(SniperItem sniperItem)
        {
            var con = new OfficialTradeWsConnection(Logger, _apiWrapper) {OnItemFound = OnItemFound};
            var ws = con.Initialize(sniperItem, _configuration.PoeSessionId);

            _webSockets.Add(ws);

            return ws;
        }

        readonly List<WebSocket> _webSockets = new List<WebSocket>();
        readonly OfficialTradeApiWrapper _apiWrapper;
        readonly OfficialTradeConfiguration _configuration;
    }
}