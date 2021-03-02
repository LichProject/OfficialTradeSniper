using LiveSearchEngine.Delegates;

namespace LiveSearchEngine.Interfaces
{
    /// <summary>
    /// Livesearch engine.
    /// </summary>
    public interface ILiveSearchEngine
    {
        /// <summary>
        /// The event will be fired if the engine finds an item from the sniper list.
        /// </summary>
        event ItemFoundDelegate ItemFound;
        
        /// <summary>
        /// Logger instance.
        /// </summary>
        ILogger Logger { get; }
        
        /// <summary>
        /// Ensure any of websocket connections has been established.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connect engine to the livesearch.
        /// You should connect OnItemFound event to the ValidationDelegate from the base class.
        /// </summary>
        /// <param name="sniperItem">Your sniper item instance.</param>
        void Connect(ISniperItem sniperItem);
        
        /// <summary>
        /// Stops the livesearch engine and disconnects all available websocket connections.
        /// </summary>
        void Close();
    }
}