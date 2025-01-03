namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    public class OfficialTradeLiveSearch : LiveSearchEngineBase<OfficialTradeConfiguration>
    {
        private const int MaxItemLimit = 10;

        public OfficialTradeLiveSearch(OfficialTradeConfiguration configuration)
            : base(configuration)
        {
            ApiWrapper = new OfficialTradeApiWrapper(configuration);
        }

        public OfficialTradeApiWrapper ApiWrapper { get; }

        /// <inheritdoc />
        public override void UseNewConfiguration(OfficialTradeConfiguration configuration)
        {
            base.UseNewConfiguration(configuration);
            ApiWrapper.UseNewConfiguration(configuration);
        }

        /// <inheritdoc />
        public override bool ValidateConfiguration() =>
            !string.IsNullOrEmpty(Configuration.PoeSessionId);

        /// <inheritdoc />
        protected override WebSocketConnection CreateWebSocket(ISniperItem sniperItem) =>
            new OfficialTradeWebSocketFactory(Configuration).Create(sniperItem);

        /// <inheritdoc />
        protected override async Task WsOnMessageReceived(ISniperItem sniperItem, string data)
        {
            DateTime receivedAt = DateTime.Now;

            if (data.Contains("auth"))
            {
                return;
            }

            Dictionary<string, Stack<string>> p = JsonConvert.DeserializeObject<Dictionary<string, Stack<string>>>(data);
            if (!p.TryGetValue("new", out Stack<string> value))
            {
                return;
            }

            FetchResponse fetchResponse;
            try
            {
                if ((fetchResponse = await ApiWrapper.FetchAsync(value, sniperItem.SearchUrlWrapper.Hash, MaxItemLimit)) == null)
                {
                    return;
                }
            }
            catch (HttpRequestException e)
            {
                RaiseErrorEvent(sniperItem, e);
                return;
            }

            foreach (Result result in fetchResponse.Result)
            {
                RaiseItemFoundEvent(sniperItem, result.Item, result.Listing, receivedAt);
            }
        }
    }
}