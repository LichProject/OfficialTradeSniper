using System;
using System.Collections.Generic;
using LiveSearchEngine.Delegates;
using WebSocketSharp;

namespace LiveSearchEngine.Interfaces
{
    /// <summary>
    /// Livesearch engine.
    /// </summary>
    public interface ILiveSearchEngine : IDisposable
    {
        event EventHandler Start;
        event EventHandler Stop;
        event ItemFoundDelegate ItemFound;
        event SniperItemConnectedStateDelegate Connected;
        event WebSocketDisconnectedDelegate Disconnected;
        event WebSocketDisconnectedDelegate Error;

        IRateLimit RateLimit { get; }

        /// <summary>
        /// Ensure any of websocket connections has been established.
        /// </summary>
        bool IsConnected { get; }

        bool ValidateConfiguration();

        List<WebSocket> ConnectAll(IEnumerable<ISniperItem> sniperItems);

        /// <summary>
        /// Connect engine to the livesearch.
        /// You should connect OnItemFound event to the ValidationDelegate from the base class.
        /// </summary>
        /// <param name="sniperItem">Your sniper item instance.</param>
        WebSocket Connect(ISniperItem sniperItem);

        void Close(WebSocket ws);

        /// <summary>
        /// Stops the livesearch engine and disconnects all available websocket connections.
        /// </summary>
        void CloseAll();
    }
}