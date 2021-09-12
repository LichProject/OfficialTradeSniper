namespace LiveSearchEngine.Models.Poe
{
    public class Property
    {
        public int DisplayMode { get; set; }
        public string Name { get; set; }
        public decimal Progress { get; set; }
        public int Type { get; set; }
        public object[][] Values { get; set; }
    }
}