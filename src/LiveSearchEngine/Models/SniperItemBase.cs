namespace LiveSearchEngine.Models
{
    public class SniperItemBase : ISniperItem
    {
        private readonly string url;
        private ISearchUrlWrapper liveUrlWrapper;

        public SniperItemBase(string description, string url)
        {
            this.url = url;
            Description = description;
        }

        public SniperItemBase()
        {
        }

        public string Description { get; set; }
        public ISearchUrlWrapper SearchUrlWrapper => liveUrlWrapper ?? (liveUrlWrapper = new LiveUrlWrapperBase(url));
    }
}