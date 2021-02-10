﻿using System.Linq;
using LiveSearchEngine.Delegates;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;
using LiveSearchEngine.Models.Poe;
using LiveSearchEngine.Models.Poe.Fetch;

namespace LiveSearchEngine.LiveSearch
{
    /// <inheritdoc/>
    public abstract class BaseLiveSearchEngine<T> : ILiveSearchEngine where T : BaseLiveSearchConfiguration
    {
        protected BaseLiveSearchEngine(ILogger logger, T configuration)
        {
            Logger = logger;
            Configuration = configuration;
        }

        public virtual void UseNewConfiguration(T configuration)
        {
            Configuration = configuration;
        }

        #region Implementation of ILiveSearchEngine

        /// <inheritdoc/>
        public event ItemFoundDelegate OnItemFound;

        /// <inheritdoc/>
        public ILogger Logger { get; }

        public abstract bool IsConnected { get; }

        protected T Configuration { get; private set; }

        public abstract void Connect(SniperItem sniperItem);
        public abstract void Close();

        #endregion

        protected void ValidationDelegate(SniperItem sniperItem, Item item, Listing listing)
        {
            if (Configuration.Validators.Any(x => !x.Validate(sniperItem, item, listing)))
                return;

            OnItemFound?.Invoke(sniperItem, item, listing);
        }
    }
}