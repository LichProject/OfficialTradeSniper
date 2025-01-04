namespace LiveSearchEngine.Interfaces
{
    /// <summary>
    /// Livesearch engine.
    /// </summary>
    public interface ILiveSearchEngine
    {
        event EventHandler Start;
        event EventHandler Stop;
        event ItemFoundDelegate ItemFound;
        event SniperItemConnectedStateDelegate Connected;
        event WebSocketDisconnectedDelegate Disconnected;
        event WebSocketErrorDelegate Error;

        /// <summary>
        /// Ensure any of websocket connections has been established.
        /// </summary>
        bool IsConnected { get; }

        bool ValidateConfiguration();

        Task<List<WebSocketConnection>> ConnectAsync(IEnumerable<ISniperItem> sniperItems, CancellationToken cancellationToken);

        /// <summary>
        /// Connect engine to the livesearch.
        /// You should connect OnItemFound event to the ValidationDelegate from the base class.
        /// </summary>
        /// <param name="sniperItem">Your sniper item instance.</param>
        Task<WebSocketConnection> ConnectAsync(ISniperItem sniperItem);

        /// <summary>
        /// Stops the livesearch engine and disconnects all available websocket connections.
        /// </summary>
        Task DisconnectAsync();
    }
}