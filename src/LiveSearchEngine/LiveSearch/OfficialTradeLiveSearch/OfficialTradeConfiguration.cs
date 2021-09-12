using LiveSearchEngine.Interfaces;

namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// Configuration of the official trade api and livesearch.
    /// </summary>
    public class OfficialTradeConfiguration : ILiveSearchConfiguration
    {
        /// <summary>
        /// POESESSID Cookie value.
        /// </summary>
        public string PoeSessionId { get; set; }
    }
}