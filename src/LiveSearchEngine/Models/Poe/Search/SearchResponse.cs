using System;

namespace LiveSearchEngine.Models.Poe.Search
{
    public class SearchResponse
    {
        public string Id { get; set; }
        public long Complexity { get; set; }
        public string[] Result { get; set; } = Array.Empty<string>();
        public long Total { get; set; }
    }
}