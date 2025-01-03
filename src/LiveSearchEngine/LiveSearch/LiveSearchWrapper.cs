namespace LiveSearchEngine.LiveSearch
{
    /// <summary>
    /// Wrapper of the livesearch engine.
    /// </summary>
    public sealed class LiveSearchWrapper<TEngine, TConfiguration> : IAsyncDisposable
        where TEngine : LiveSearchEngineBase<TConfiguration>
        where TConfiguration : ILiveSearchConfiguration
    {
        private readonly List<ISniperItem> sniperItems = [];

        public event EventHandler Start
        {
            add => Engine.Start += value;
            remove => Engine.Start -= value;
        }

        public event EventHandler Stop
        {
            add => Engine.Stop += value;
            remove => Engine.Stop -= value;
        }

        public event SniperItemConnectedStateDelegate Connected
        {
            add => Engine.Connected += value;
            remove => Engine.Connected -= value;
        }

        public event WebSocketDisconnectedDelegate Disconnected
        {
            add => Engine.Disconnected += value;
            remove => Engine.Disconnected -= value;
        }

        public event WebSocketErrorDelegate Error
        {
            add => Engine.Error += value;
            remove => Engine.Error -= value;
        }

        public event ItemFoundDelegate ItemFound
        {
            add => Engine.ItemFound += value;
            remove => Engine.ItemFound -= value;
        }

        public LiveSearchWrapper(TConfiguration cfg)
        {
            Engine = (TEngine)Activator.CreateInstance(typeof(TEngine), cfg);
        }

        public LiveSearchWrapper(TEngine engine)
        {
            Engine = engine;
        }

        /// <inheritdoc cref="ILiveSearchEngine.IsConnected"/>
        public bool IsRunning => Engine.IsConnected;

        public bool Stopped { get; private set; } = true;

        /// <inheritdoc cref="ILiveSearchEngine"/>
        public TEngine Engine { get; }

        /// <summary>
        /// Clear the sniper list.
        /// </summary>
        public void RemoveAllSniperItems() =>
            sniperItems.Clear();

        /// <summary>
        /// Add sniper items to the livesearch.
        /// </summary>
        public void AddSniperItems(IEnumerable<ISniperItem> sniperItems) =>
            this.sniperItems.AddRange(sniperItems);

        /// <summary>
        /// Add a sniper item to the livesearch.
        /// </summary>
        public void AddSniperItem(ISniperItem sniperItem) =>
            sniperItems.Add(sniperItem);

        /// <summary>
        /// Run the livesearch engine (establish connections and start receiving websocket messages).
        /// </summary>
        /// <exception cref="InvalidOperationException">Sniper-list isn't enitialized (use SetSniperList method).</exception>
        public async Task<RunError> ConnectAsync(CancellationToken cancellationToken)
        {
            if (!sniperItems.Any())
            {
                return RunError.NoItems;
            }

            if (Engine.IsConnected)
            {
                return RunError.AlreadyConnected;
            }

            if (!Engine.ValidateConfiguration())
            {
                return RunError.InvalidConfiguration;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return RunError.None;
            }

            try
            {
                Stopped = false;
                await Engine.DisconnectAsync(sniperItems, cancellationToken);

                return RunError.None;
            }
            catch
            {
                Stopped = true;
                throw;
            }
        }

        /// <inheritdoc cref="ILiveSearchEngine.DisconnectAsync"/>
        public async Task DisconnectAsync()
        {
            try
            {
                await Engine.DisconnectAsync();
            }
            finally
            {
                Stopped = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Engine == null)
            {
                return;
            }

            await Engine.DisconnectAsync();
        }
    }
}