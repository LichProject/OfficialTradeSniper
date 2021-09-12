using LiveSearchEngine.Interfaces;

namespace LiveSearchEngine.Models
{
    public class SniperItemBase : ISniperItem
    {
        readonly string _url;

        public SniperItemBase(string description, string url)
        {
            _url = url;
            Description = description;
        }

        public SniperItemBase()
        {
        }

        public string Description { get; set; }
        public ISearchUrlWrapper SearchUrlWrapper => _liveUrlWrapper ?? (_liveUrlWrapper = new LiveUrlWrapperBase(_url));

        ISearchUrlWrapper _liveUrlWrapper;
    }
}