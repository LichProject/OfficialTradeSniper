namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// WebSocket connection to the official trade site.
    /// </summary>
    public class OfficialTradeWebSocketFactory
    {
        private readonly OfficialTradeConfiguration configuration;
        private WebSocketConnection connection;

        public OfficialTradeWebSocketFactory(OfficialTradeConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public WebSocketConnection Create(ISniperItem sniperItem)
        {
            if (connection != null)
            {
                return connection;
            }

            string webSocketUrl = (configuration.UsePoe2Api
                                      ? OfficialTradeConstants.OfficialTradeApiUrlPoe2
                                      : OfficialTradeConstants.OfficialTradeApiUrl)
                                  .Replace("https", "wss")
                                  + $"/{sniperItem.SearchUrlWrapper.WebSocketRelative.Replace("live", $"{(configuration.UsePoe2Api ? "live/poe2" : "live")}")}";

            Uri uri = new(webSocketUrl);

            ClientWebSocket clientWebSocket = new();
            clientWebSocket.Options.Cookies = new();

            clientWebSocket.Options.Cookies.Add(new Cookie(
                OfficialTradeConstants.PoeSessionIdCookieName,
                configuration.PoeSessionId,
                "/", ".pathofexile.com"));

            clientWebSocket.Options.SetRequestHeader("User-Agent", configuration.UserAgent);
            clientWebSocket.Options.SetRequestHeader("Origin", OfficialTradeConstants.OfficialSiteUrl);

            return connection = new(sniperItem, clientWebSocket, uri);
        }
    }
}