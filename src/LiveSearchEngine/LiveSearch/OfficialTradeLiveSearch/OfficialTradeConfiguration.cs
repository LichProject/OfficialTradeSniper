namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    public class OfficialTradeConfiguration : ILiveSearchConfiguration
    {
        public string PoeSessionId { get; set; }
        public string UserAgent { get; set; }
        public bool UsePoe2Api { get; set; }
    }
}