namespace LiveSearchEngine.Models.Poe
{
    public class Socket
    {
        [JsonProperty("attr")]
        public string Attribute { get; set; }

        public int Group { get; set; }

        [JsonProperty("sColour")]
        public string Colour { get; set; }
    }
}