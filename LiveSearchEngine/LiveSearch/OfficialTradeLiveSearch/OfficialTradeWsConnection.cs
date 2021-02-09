using System;
using System.Collections.Generic;
using LiveSearchEngine.Delegates;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// WebSocket connection to the official trade site.
    /// </summary>
    public class OfficialTradeWsConnection
    {
        const int MaxItemLimit = 10;

        /// <summary>
        /// <inheritdoc cref="ILiveSearchEngine.OnItemFound"/>
        /// </summary>
        public ItemFoundDelegate OnItemFound;

        public OfficialTradeWsConnection(ILogger logger, OfficialTradeApiWrapper apiWrapper)
        {
            _logger = logger;
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
        public WebSocket Initialize(SniperItem sniperItem, string poeSessionId)
        {
            if (_ws != null)
                throw new InvalidOperationException("already initialized.");

            _sniperItem = sniperItem;

            _ws = new WebSocket(_sniperItem.LiveUrlWrapper.WebSocketUrl) {Compression = CompressionMethod.Deflate};
            _ws.SetCookie(new Cookie(GlobalConstants.PoeSessionIdCookieName, poeSessionId));

            _ws.Origin = GlobalConstants.OfficialSiteUrl;
            _ws.OnOpen += OnOpen;
            _ws.OnError += OnError;
            _ws.OnMessage += OnMessage;
            _ws.OnClose += OnClose;

            return _ws;
        }

        void OnOpen(object sender, EventArgs e)
        {
            IsConnected = true;
            _logger.Info($"[INFO] Успешно подключились к {_sniperItem.LiveUrlWrapper.SearchUrl}");
        }

        void OnClose(object sender, CloseEventArgs e)
        {
            IsConnected = false;
            string message = $"Подключение к {_sniperItem.LiveUrlWrapper.SearchUrl} было закрыто, код: {e.Code}";
            switch (e.Code)
            {
                case 1000:
                    _logger.Warn(message + ", Причина: Неверный POESSID.");
                    break;

                case 1005:
                    _logger.Warn(message + ", Причина: По запросу пользователя.");
                    break;

                default:
                    _logger.Warn(message + ", Причина: Неизвестно.");
                    break;
            }
        }

        void OnError(object sender, ErrorEventArgs e)
        {
            _logger.Error($"[{_sniperItem.LiveUrlWrapper.SearchUrl}]\n{e.Exception}");
        }

        void OnMessage(object sender, MessageEventArgs e)
        {
            var data = e.Data;
            if (!_authenticated && data.Contains("auth"))
            {
                _authenticated = true;
                return;
            }

            var p = JsonConvert.DeserializeObject<Dictionary<string, Stack<string>>>(data);
            if (!p.ContainsKey("new"))
            {
                _logger.Error($"Неизвестное сообщение: {p}");
                return;
            }

            var fetchResponse = _apiWrapper.FetchAsync(p["new"], _sniperItem.LiveUrlWrapper.Hash, MaxItemLimit).GetAwaiter().GetResult();
            if (fetchResponse == null)
            {
                _logger.Warn("FetchResponse is null (you got requests timeout?)");
                return;
            }

            foreach (var result in fetchResponse.Result)
            {
                OnItemFound?.Invoke(_sniperItem, result.Item, result.Listing);
            }
        }

        bool _authenticated;
        readonly ILogger _logger;
        readonly OfficialTradeApiWrapper _apiWrapper;

        WebSocket _ws;
        SniperItem _sniperItem;
    }
}