namespace LiveSearchEngine.Models.Poe.Search
{
    public class ExchangeSectionFilterHtml
    {
        public ExchangeHtml Exchange { get; set; }
        public string Status { get; set; }
    }

    public class ExchangeHtml
    {
        public Dictionary<string, bool> Have { get; set; }
        public Dictionary<string, bool> Want { get; set; }
        public ExchangeStock Stock { get; set; }
    }

    public class ExchangeStock
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }
}