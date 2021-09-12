namespace LiveSearchEngine.Models.Poe.Search
{
    public class ExchangeRequest
    {
        public Exchange Exchange { get; set; }
    }

    public class Exchange
    {
        public string[] Have { get; set; } 
        public string[] Want { get; set; }
        public int Minimum { get; set; }
        public Status Status { get; set; }
    }
}