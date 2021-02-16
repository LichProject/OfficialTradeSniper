using System;
using System.Collections.Generic;
using System.Linq;
using LiveSearchEngine.Delegates;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;

namespace LiveSearchEngine.LiveSearch
{
    /// <summary>
    /// Wrapper of the livesearch engine.
    /// </summary>
    public sealed class LiveSearchWrapper
    {
        public event EventHandler OnStart;
        public event EventHandler OnStop;

        public LiveSearchWrapper(ILiveSearchEngine engine)
        {
            Engine = engine;
        }

        /// <inheritdoc cref="ILiveSearchEngine.IsConnected"/>
        public bool IsRunning => Engine.IsConnected;

        /// <inheritdoc cref="ILiveSearchEngine"/>
        public ILiveSearchEngine Engine { get; }

        /// <inheritdoc cref="ILiveSearchEngine.Logger"/>
        public ILogger Logger => Engine.Logger;

        /// <summary>
        /// Set the sniper list for the livesearch engine.
        /// </summary>
        public void SetSniperList(IEnumerable<SniperItem> sniperItems)
        {
            _sniperItems = sniperItems;
        }

        /// <summary>
        /// Run the livesearch engine (establish connections and start receiving websocket messages).
        /// </summary>
        /// <exception cref="InvalidOperationException">Sniper-list isn't enitialized (use SetSniperList method).</exception>
        public bool Run()
        {
            if (!_sniperItems.Any())
                return false;

            if (Engine.IsConnected)
                return false;

            foreach (var si in _sniperItems)
            {
                Engine.Connect(si);
                Logger.Info($"[LiveSearch::Run] <{si.Description}> {si.LiveUrlWrapper.SearchUrl}");
            }

            OnStart?.Invoke(this, null);

            return true;
        }

        /// <inheritdoc cref="ILiveSearchEngine.Close"/>
        public void Stop()
        {
            Engine.Close();
            OnStop?.Invoke(this, null);
        }

        /// <summary>
        /// Subscribe the delegate to the OnItemFound event.
        /// </summary>
        public void Subscribe(ItemFoundDelegate @delegate)
        {
            Engine.ItemFound += @delegate;
        }

        IEnumerable<SniperItem> _sniperItems;
    }
}