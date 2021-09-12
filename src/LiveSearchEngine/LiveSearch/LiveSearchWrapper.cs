using System;
using System.Collections.Generic;
using System.Linq;
using LiveSearchEngine.Delegates;
using LiveSearchEngine.Enums;
using LiveSearchEngine.Interfaces;

namespace LiveSearchEngine.LiveSearch
{
    /// <summary>
    /// Wrapper of the livesearch engine.
    /// </summary>
    public sealed class LiveSearchWrapper<TEngine, TConfiguration> : IDisposable where TEngine : LiveSearchEngineBase<TConfiguration>
                                                                                 where TConfiguration : ILiveSearchConfiguration
    {
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
        
        public event SniperItemConnectedStateDelegate Reconnected
        {
            add => Engine.Reconnected += value;
            remove => Engine.Reconnected -= value;
        }

        public event WebSocketDisconnectedDelegate Disconnected
        {
            add => Engine.Disconnected += value;
            remove => Engine.Disconnected -= value;
        }

        public event WebSocketDisconnectedDelegate Error
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
        public void RemoveAllSniperItems()
        {
            _sniperItems.Clear();
        }

        /// <summary>
        /// Add sniper items to the livesearch.
        /// </summary>
        public void AddSniperItems(IEnumerable<ISniperItem> sniperItems)
        {
            _sniperItems.AddRange(sniperItems);
        }

        /// <summary>
        /// Add a sniper item to the livesearch.
        /// </summary>
        public void AddSniperItem(ISniperItem sniperItem)
        {
            _sniperItems.Add(sniperItem);
        }

        /// <summary>
        /// Run the livesearch engine (establish connections and start receiving websocket messages).
        /// </summary>
        /// <exception cref="InvalidOperationException">Sniper-list isn't enitialized (use SetSniperList method).</exception>
        public RunError ConnectAll()
        {
            if (!_sniperItems.Any())
                return RunError.NoItems;

            if (Engine.IsConnected)
                return RunError.AlreadyConnected;

            if (!Engine.ValidateConfiguration())
                return RunError.InvalidConfiguration;

            Engine.ConnectAll(_sniperItems);

            Stopped = false;

            return RunError.None;
        }

        /// <inheritdoc cref="ILiveSearchEngine.CloseAll"/>
        public void StopAll()
        {
            Stopped = true;
            Engine.CloseAll();
        }

        readonly List<ISniperItem> _sniperItems = new List<ISniperItem>();

        #region IDisposable

        public void Dispose()
        {
            Engine?.Dispose();
        }

        #endregion
    }
}