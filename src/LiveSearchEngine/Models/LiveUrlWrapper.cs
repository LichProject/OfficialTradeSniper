namespace LiveSearchEngine.Models
{
    public class LiveUrlWrapper
    {
        public LiveUrlWrapper(string hash, string league)
        {
            Hash = hash;
            League = league;
        }

        public string Hash { get; }
        public string League { get; }

        public string SearchUrl => $"{GlobalConstants.OfficialTradeUrl}/search/{League}/{Hash}";
        public string WebSocketUrl => $"{GlobalConstants.OfficialTradeApiUrl.Replace("https", "wss")}/live/{League}/{Hash}";
    }
}