namespace LiveSearchEngine.Models.Poe.Fetch
{
    public class Listing
    {
        public DateTime Indexed { get; set; }
        public string Method { get; set; }
        public Account Account { get; set; }
        public Price Price { get; set; }
        public List<Price> Offers { get; set; } = new List<Price>();
        public Stash Stash { get; set; }
        public string Whisper { get; set; }
        
        [JsonProperty("whisper_token")]
        public string WhisperToken { get; set; }
    }
}