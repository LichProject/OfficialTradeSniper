namespace LiveSearchEngine.Models.Poe.Search
{
    public class ExchangeRequest
    {
        public string Engine { get; set; }
        public Exchange Query { get; set; }
        public ExchangeSort Sort { get; set; }
    }

    public class ExchangeSort
    {
        public string Have { get; set; }
        public string Want { get; set; }
    }
    
    public class Exchange
    {
        public string[] Have { get; set; } 
        public string[] Want { get; set; }
        public int Minimum { get; set; }
        public Status Status { get; set; }
    }
}