namespace LiveSearchEngine.LiveSearch
{
    public abstract class LiveSearchEngineBase<T> : ILiveSearchEngine, IAsyncDisposable
        where T : ILiveSearchConfiguration
    {
        private readonly List<WebSocketConnection> webSockets = [];

        private bool started;

        protected LiveSearchEngineBase(T configuration)
        {
            Configuration = configuration;
            Start += (_, __) => started = true;
            Stop += (_, __) => started = false;
        }

        public virtual void UseNewConfiguration(T configuration) =>
            Configuration = configuration;

        public event EventHandler Start;
        public event EventHandler Stop;
        public event ItemFoundDelegate ItemFound;
        public event SniperItemConnectedStateDelegate Connected;
        public event WebSocketDisconnectedDelegate Disconnected;
        public event WebSocketErrorDelegate Error;

        protected void RaiseStopEvent() => Stop?.Invoke(this, EventArgs.Empty);
        protected void RaiseStartEvent() => Start?.Invoke(this, EventArgs.Empty);

        protected void RaiseItemFoundEvent(ISniperItem sniperItem, Item item, Listing listing, DateTime receivedAt) =>
            ItemFound?.Invoke(sniperItem, item, listing, receivedAt);

        protected void RaiseConnectedEvent(ISniperItem sniperItem) => Connected?.Invoke(sniperItem);
        protected void RaiseDisconnectedEvent(ISniperItem sniperItem, string reason) => Disconnected?.Invoke(sniperItem, reason);
        protected void RaiseErrorEvent(ISniperItem sniperItem, Exception exception) => Error?.Invoke(sniperItem, exception);

        protected T Configuration { get; private set; }

        public abstract bool ValidateConfiguration();
        protected abstract WebSocketConnection CreateWebSocket(ISniperItem sniperItem);

        public virtual bool IsConnected => webSockets.ToList().Any(x => x.IsConnected);

        public virtual async Task<List<WebSocketConnection>> ConnectAsync(IEnumerable<ISniperItem> sniperItems, CancellationToken cancellationToken)
        {
            AsyncPolicyWrap policies = CreateWebSocketConnectionPolicies();
            List<WebSocketConnection> websockets = [];

            bool connected = false;

            foreach (ISniperItem item in sniperItems.ToArray())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await policies.ExecuteAsync(
                    async () =>
                    {
                        WebSocketConnection ws = await ConnectAsync(item);
                        if (ws == null)
                        {
                            connected = false;
                            return;
                        }

                        websockets.Add(ws);
                        connected = true;
                    });
            }

            if (cancellationToken.IsCancellationRequested || !connected)
            {
                await Task.WhenAll(websockets.Select(x => x.CloseAsync()));
                return [];
            }

            RaiseStartEvent();

            return websockets;
        }

        // todo refactor it
        public bool ShouldReconnect(ISniperItem sniperItem)
        {
            WebSocketConnection existingConnection = webSockets.FirstOrDefault(x => x.SniperItem.Equals(sniperItem));
            return existingConnection == null
                   || (!existingConnection.IsConnected
                       && existingConnection.State != WebSocketState.Connecting
                       && existingConnection.State != WebSocketState.CloseSent);
        }

        public virtual async Task<WebSocketConnection> ConnectAsync(ISniperItem sniperItem)
        {
            WebSocketConnection existingConnection = webSockets.FirstOrDefault(x => x.SniperItem.Equals(sniperItem));
            if (existingConnection != null)
            {
                if (existingConnection.IsConnected
                    || existingConnection.State == WebSocketState.Connecting
                    || existingConnection.State == WebSocketState.CloseSent)
                {
                    return existingConnection;
                }

                try
                {
                    await existingConnection.CloseAsync();
                }
                catch
                {
                }

                webSockets.Remove(existingConnection);
            }

            WebSocketConnection ws = CreateWebSocket(sniperItem);
            ws.OnClose += reason => WsOnClosed(ws, sniperItem, reason);
            ws.OnError += exception => WsOnError(ws, sniperItem, exception);
            ws.OnMessage += data => WsOnMessageReceivedInternal(sniperItem, data);

            webSockets.Add(ws);

            if (!await ws.ConnectAsync())
            {
                return null;
            }

            RaiseConnectedEvent(sniperItem);

            return ws;
        }

        public virtual async Task DisconnectAsync()
        {
            foreach (WebSocketConnection ws in webSockets.ToList())
            {
                await ws.CloseAsync();
                webSockets.Remove(ws);
            }

            RaiseStopEvent();
        }

        private async Task WsOnMessageReceivedInternal(ISniperItem sniperItem, string data)
        {
            if (!started)
            {
                return;
            }

            await WsOnMessageReceived(sniperItem, data);
        }

        protected abstract Task WsOnMessageReceived(ISniperItem sniperItem, string data);

        protected virtual Task WsOnError(WebSocketConnection ws, ISniperItem sniperItem, Exception e)
        {
            RaiseErrorEvent(sniperItem, e);
            return Task.CompletedTask;
        }

        protected virtual Task WsOnClosed(WebSocketConnection ws, ISniperItem sniperItem, string reason)
        {
            RaiseDisconnectedEvent(sniperItem, reason);
            return Task.CompletedTask;
        }

        private static AsyncPolicyWrap CreateWebSocketConnectionPolicies()
        {
            AsyncRetryPolicy retryRateLimitPolicy = Policy.Handle<RateLimitRejectedException>().RetryForeverAsync();

            return PolicyWrap.WrapAsync(
                retryRateLimitPolicy,
                RateLimitPolicy.RateLimitAsync(10, TimeSpan.FromSeconds(10), maxBurst: 10));
        }

        public async ValueTask DisposeAsync()
        {
            webSockets.ForEach(x => x.Dispose());
            await DisconnectAsync();
        }
    }
}