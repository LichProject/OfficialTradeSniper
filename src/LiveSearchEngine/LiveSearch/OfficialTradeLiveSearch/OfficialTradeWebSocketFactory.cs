using System;
using System.Collections.Generic;
using LiveSearchEngine.Delegates;
using LiveSearchEngine.Interfaces;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// WebSocket connection to the official trade site.
    /// </summary>
    public class OfficialTradeWebSocketFactory
    {
        const int MaxItemLimit = 10;

        /// <summary>
        /// <inheritdoc cref="ILiveSearchEngine.ItemFound"/>
        /// </summary>
        public ItemFoundDelegate ItemFound;

        public WebSocketDisconnectedDelegate Disconnected;
        public WebSocketDisconnectedDelegate Error;

        public OfficialTradeWebSocketFactory(OfficialTradeApiWrapper apiWrapper)
        {
            _apiWrapper = apiWrapper;
        }

        /// <summary>
        /// Checks if a connection has been established.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Initialize the connection.
        /// </summary>
        /// <param name="sniperItem">What do you want to snipe.</param>
        /// <param name="poeSessionId">POESESSID cookie value.</param>
        /// <exception cref="InvalidOperationException">Connection has already been established.</exception>
        public WebSocket Create(ISniperItem sniperItem, string poeSessionId)
        {
            if (_ws != null)
                throw new InvalidOperationException("initialized recently.");

            _sniperItem = sniperItem;

            var cookies = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(GlobalConstants.PoeSessionIdCookieName, poeSessionId)
            };

            _ws = new WebSocket(
                sniperItem.SearchUrlWrapper.WebSocketUrl,
                cookies: cookies,
                origin: GlobalConstants.OfficialSiteUrl,
                userAgent: "websocket-client");

            ReSubscribe();

            return _ws;
        }

        void ReSubscribe()
        {
            _ws.Opened += WsOnOpened;
            _ws.Closed += WsOnClosed;
            _ws.Error += WsOnError;
            _ws.MessageReceived += WsOnMessageReceived;
        }

        void WsOnError(object sender, ErrorEventArgs e)
        {
            IsConnected = false;
            Error?.Invoke(_sniperItem, e.Exception);
        }

        void WsOnClosed(object sender, EventArgs e)
        {
            IsConnected = false;
            Disconnected?.Invoke(_sniperItem, null);
        }

        void WsOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var response = e.Message;
            if (!_authenticated && response.Contains("auth"))
            {
                _authenticated = true;
                return;
            }

            var p = JsonConvert.DeserializeObject<Dictionary<string, Stack<string>>>(response);
            if (!p.ContainsKey("new"))
                return;

            var fetchResponse = _apiWrapper.Fetch(p["new"], _sniperItem.SearchUrlWrapper.Hash, MaxItemLimit);
            if (fetchResponse == null)
                return;

            foreach (var result in fetchResponse.Result)
            {
                ItemFound?.Invoke(_sniperItem, result.Item, result.Listing);
            }
        }

        void WsOnOpened(object sender, EventArgs e)
        {
            IsConnected = true;
        }

        bool _authenticated;
        readonly OfficialTradeApiWrapper _apiWrapper;

        WebSocket _ws;
        ISniperItem _sniperItem;
    }
}