namespace LiveSearchEngine.Models.Poe
{
    public class Price
    {
        /// <summary>
        /// Currency name (from display note).
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Currency amount per one item.
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Offer type (b/o, exact etc).
        /// </summary>
        public string Type { get; set; }
        
        public int Stock { get; set; }
        
        public Price Exchange { get; set; }
        public Price Item { get; set; }

        public bool IsExchangeSection => Exchange != null;
    }
}