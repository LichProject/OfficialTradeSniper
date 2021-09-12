using System;
using LiveSearchEngine.Interfaces;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// WebSocket connection to the official trade site.
    /// </summary>
    public class OfficialTradeWebSocketFactory
    {
        /// <summary>
        /// Initialize the connection.
        /// </summary>
        /// <param name="sniperItem">What do you want to snipe.</param>
        /// <param name="poeSessionId">POESESSID cookie value.</param>
        /// <exception cref="InvalidOperationException">Connection has already been established.</exception>
        public WebSocket Create(ISniperItem sniperItem, string poeSessionId)
        {
            if (_ws != null)
                return _ws;
            
            _ws = new WebSocket(sniperItem.SearchUrlWrapper.WebSocketUrl); 
            _ws.SetCookie(new Cookie(OfficialTradeConstants.PoeSessionIdCookieName, poeSessionId));

            _ws.Log.Level = LogLevel.Debug;
            _ws.Compression = CompressionMethod.Deflate;
            _ws.Origin = OfficialTradeConstants.OfficialSiteUrl;
            
            return _ws;
        }

        WebSocket _ws;
    }
}