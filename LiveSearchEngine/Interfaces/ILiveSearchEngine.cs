using LiveSearchEngine.Delegates;
using LiveSearchEngine.Models;

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
        event ItemFoundDelegate OnItemFound;
        
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
        /// </summary>
        /// <param name="sniperItem">Your sniper item instance.</param>
        void Connect(SniperItem sniperItem);
        
        /// <summary>
        /// Stops the livesearch engine and disconnects all available websocket connections.
        /// </summary>
        void Close();
    }
}