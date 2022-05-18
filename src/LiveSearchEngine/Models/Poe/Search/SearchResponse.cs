using System;
using System.Collections.Generic;
using LiveSearchEngine.Models.Poe.Fetch;

namespace LiveSearchEngine.Models.Poe.Search
{
    public class SearchResponseExchange : SearchResponse
    {
        public new Dictionary<string, Result> Result { get; set; } =
            new Dictionary<string, Result>();
    }
    
    public class SearchResponse
    {
        public string Id { get; set; }
        public long Complexity { get; set; }
        public string[] Result { get; set; } = Array.Empty<string>();
        public long Total { get; set; }
    }
}