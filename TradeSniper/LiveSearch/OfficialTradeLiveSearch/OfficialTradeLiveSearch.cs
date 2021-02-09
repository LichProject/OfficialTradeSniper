using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TradeSniper.Delegates;
using TradeSniper.Interfaces;
using WebSocketSharp;
using WebSocketSharp.Net;
using LogLevel = TradeSniper.Enums.LogLevel;

namespace TradeSniper.LiveSearch
{
    public class OfficialTradeLiveSearch : IWebSocketConnectable
    {
        public OfficialTradeLiveSearch(string liveUrl, string poessid)
        {
            _liveUrl = liveUrl;
            _poessid = poessid;
        }

        ~OfficialTradeLiveSearch()
        {
            Close();
        }

        #region Events

        public event ItemFoundDelegate OnItemFound;
        public event LogMessageDelegate OnLogMessage;

        #endregion

        public WebSocket WebSocket { get; private set; }

        public bool IsConnected { get; private set; }
        public int MaxItemLimit { get; } = 10;

        #region Implementation of IWebSocketConnectable

        public void Connect()
        {
            Configure();
            WebSocket.Connect();
        }

        public void Close()
        {
            if (IsConnected)
                WebSocket.Close();
        }

        #endregion

        void Configure()
        {
            if (WebSocket != null)
                return;

            WebSocket = new WebSocket(_liveUrl) {Compression = CompressionMethod.Deflate};
            WebSocket.SetCookie(new Cookie("POESESSID", _poessid));
            WebSocket.Origin = "https://www.pathofexile.com";
            WebSocket.OnOpen += OnOpen;
            WebSocket.OnError += OnError;
            WebSocket.OnMessage += OnMessage;
            WebSocket.OnClose += OnClose;
        }

        #region WebSocket

        void OnOpen(object sender, EventArgs e)
        {
            IsConnected = true;
            OnLogMessage?.Invoke(LogLevel.Info, $"[INFO] Успешно подключились к {_liveUrl}");
        }

        void OnClose(object sender, CloseEventArgs e)
        {
            IsConnected = false;
            string message = $"Подключение к {_liveUrl} было закрыто, код: {e.Code}";
            switch (e.Code)
            {
                case 1000:
                    OnLogMessage?.Invoke(LogLevel.Warn, message + ", Причина: Неверный POESSID.");
                    break;

                case 1005:
                    OnLogMessage?.Invoke(LogLevel.Warn, message + ", Причина: По запросу пользователя.");
                    break;

                default:
                    OnLogMessage?.Invoke(LogLevel.Warn, message + ", Причина: Неизвестно.");
                    break;
            }
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
            if (!p.ContainsKey("new") && p["new"].Count == 0)
            {
                OnLogMessage?.Invoke(LogLevel.Error, $"Неизвестное сообщение: {p}");
            }

            this.GetNewItems(p["new"]);
        }

        #endregion

        bool _authenticated;
        
        readonly string _liveUrl;
        readonly string _poessid;
    }
}