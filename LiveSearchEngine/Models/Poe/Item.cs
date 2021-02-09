using Newtonsoft.Json;

namespace LiveSearchEngine.Models.Poe
{
    public class Item
    {
        public int? StackSize { get; set; }
        public string League { get; set; }
        public string Name { get; set; }

        [JsonProperty("typeLine")]
        public string BaseType { get; set; }
    }
}