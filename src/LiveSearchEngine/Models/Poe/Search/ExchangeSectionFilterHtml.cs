using System.Collections.Generic;

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
        public int Minimum { get; set; }
    }
}