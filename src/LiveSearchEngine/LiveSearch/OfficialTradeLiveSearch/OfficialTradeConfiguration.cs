using LiveSearchEngine.Interfaces;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// Configuration of the official trade api and livesearch.
    /// </summary>
    public class OfficialTradeConfiguration : ILiveSearchConfiguration
    {
        public OfficialTradeConfiguration(string poeSessionId)
        {
            PoeSessionId = poeSessionId;
        }

        public OfficialTradeConfiguration()
        {
        }

        /// <summary>
        /// POESESSID Cookie value.
        /// </summary>
        public string PoeSessionId { get; set; }
        
        public string UserAgent { get; set; }
    }
}