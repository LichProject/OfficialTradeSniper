namespace LiveSearchEngine.LiveSearch.OfficialTradeLiveSearch
{
    /// <summary>
    /// Configuration of the official trade api and livesearch.
    /// </summary>
    public class OfficialTradeConfiguration : BaseLiveSearchConfiguration
    {
        /// <summary>
        /// POESESSID Cookie value.
        /// </summary>
        public string PoeSessionId { get; set; }

        /// <summary>
        /// Delay factor for the https requests rate limit (PerRequestsDelayMs * Factor)
        /// </summary>
        public double DelayFactor { get; set; } = 1;
    }
}