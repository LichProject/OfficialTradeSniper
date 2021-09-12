using System;
using System.Collections.Generic;
using System.Linq;
using LiveSearchEngine.Delegates;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;
using WebSocketSharp;

namespace LiveSearchEngine.LiveSearch
{
    public abstract class LiveSearchEngineBase<T> : ILiveSearchEngine where T : ILiveSearchConfiguration
    {
        protected LiveSearchEngineBase(T configuration)
        {
            Configuration = configuration;
            Start += (_, __) => _started = true;
            Stop += (_, __) => _started = false;
        }

        protected LiveSearchEngineBase(T configuration, IRateLimit rateLimit)
            : this(configuration)
        {
            RateLimit = rateLimit;
        }

        ~LiveSearchEngineBase()
        {
            Dispose();
        }

        public virtual void UseNewConfiguration(T configuration)
        {
            Configuration = configuration;
        }

        public int ReconnectAttempts { get; set; } = 5;

        #region Implementation of ILiveSearchEngine

        public event EventHandler Start;
        public event EventHandler Stop;
        public event ItemFoundDelegate ItemFound;
        public event SniperItemConnectedStateDelegate Connected;
        public event SniperItemConnectedStateDelegate Reconnected;
        public event WebSocketDisconnectedDelegate Disconnected;
        public event WebSocketDisconnectedDelegate Error;

        protected void RaiseStopEvent() => Stop?.Invoke(this, EventArgs.Empty);
        protected void RaiseStartEvent() => Start?.Invoke(this, EventArgs.Empty);

        protected void RaiseItemFoundEvent(ISniperItem sniperItem, Item item, Listing listing) =>
            ItemFound?.Invoke(sniperItem, item, listing);

        protected void RaiseConnectedEvent(ISniperItem sniperItem) => Connected?.Invoke(sniperItem);
        protected void RaiseReconnectedEvent(ISniperItem sniperItem) => Reconnected?.Invoke(sniperItem);
        protected void RaiseDisconnectedEvent(ISniperItem sniperItem, Exception exception) => Disconnected?.Invoke(sniperItem, exception);
        protected void RaiseErrorEvent(ISniperItem sniperItem, Exception exception) => Error?.Invoke(sniperItem, exception);

        public IRateLimit RateLimit { get; }
        protected T Configuration { get; private set; }

        public abstract bool ValidateConfiguration();
        protected abstract WebSocket CreateWebSocket(ISniperItem sniperItem);

        public virtual bool IsConnected => _webSockets.ToList().Any(x => x.ReadyState == WebSocketState.Open);

        public virtual List<WebSocket> ConnectAll(IEnumerable<ISniperItem> sniperItems)
        {
            var websockets = new List<WebSocket>();

            foreach (var si in sniperItems)
            {
                var ws = Connect(si);
                websockets.Add(ws);
                RateLimit?.Wait();
            }

            RaiseStartEvent();

            _canReconnect = true;

            return websockets;
        }

        public virtual WebSocket Connect(ISniperItem sniperItem)
        {
            var ws = CreateWebSocket(sniperItem);
            ws.OnClose += (_, args) => WsOnClosed(ws, sniperItem, args);
            ws.OnError += (_, args) => WsOnError(ws, sniperItem, args);
            ws.OnMessage += (_, args) => WsOnMessageReceivedInternal(sniperItem, args);

            _webSockets.Add(ws);

            ws.Connect();
            RaiseConnectedEvent(sniperItem);

            return ws;
        }

        public virtual void CloseAll()
        {
            _canReconnect = false;

            foreach (var ws in _webSockets.ToList())
            {
                Close(ws);
            }

            RaiseStopEvent();
        }

        public virtual void Close(WebSocket ws)
        {
            ws.Close();
            _webSockets.Remove(ws);
        }

        #endregion

        #region WebSocket

        void WsOnMessageReceivedInternal(ISniperItem sniperItem, MessageEventArgs e)
        {
            if (!_started)
                return;

            WsOnMessageReceived(sniperItem, e);
        }

        protected abstract void WsOnMessageReceived(ISniperItem sniperItem, MessageEventArgs e);

        protected virtual void WsOnError(WebSocket ws, ISniperItem sniperItem, ErrorEventArgs e)
        {
            RaiseErrorEvent(sniperItem, e.Exception);
            if (EnsureAuthorized(e.Exception))
            {
                Reconnect(ws, sniperItem);
            }
        }

        protected virtual void WsOnClosed(WebSocket ws, ISniperItem sniperItem, EventArgs e)
        {
            RaiseDisconnectedEvent(sniperItem, null);
            Reconnect(ws, sniperItem);
        }

        void Reconnect(WebSocket ws, ISniperItem sniperItem)
        {
            if (!_canReconnect)
                return;

            if (!_reconnectCounter.TryGetValue(sniperItem, out int attempts))
                _reconnectCounter[sniperItem] = 0;

            if (attempts >= ReconnectAttempts)
                return;

            _reconnectCounter[sniperItem]++;
            
            ws.Connect();
            RaiseReconnectedEvent(sniperItem);
        }

        bool EnsureAuthorized(Exception exception)
        {
            var message = exception?.Message;
            return message == null || !message.Contains("Unauthorized");
        }

        #endregion

        bool _started;
        bool _canReconnect = true;

        readonly Dictionary<ISniperItem, int> _reconnectCounter = new Dictionary<ISniperItem, int>();
        readonly List<WebSocket> _webSockets = new List<WebSocket>();

        #region IDisposable

        public void Dispose()
        {
            CloseAll();
        }

        #endregion
    }
}