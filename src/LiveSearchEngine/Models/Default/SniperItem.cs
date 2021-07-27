using System.ComponentModel;
using LiveSearchEngine.Interfaces;
using Newtonsoft.Json;

namespace LiveSearchEngine.Models.Default
{
    public class SniperItem : ISniperItem
    {
        public SniperItem(string description, string searchHash, string league)
        {
            Description = description;
            SearchHash = searchHash;
            League = league;
        }

        public SniperItem()
        {
        }

        [Description("Описание")]
        public string Description { get; set; }

        [JsonIgnore]
        public ISearchUrlWrapper SearchUrlWrapper => _liveUrlWrapper ?? (_liveUrlWrapper = new LiveUrlWrapper(this));

        [Description("Хеш-код из поиска")]
        public string SearchHash { get; set; }

        [Description("Лига")]
        public string League { get; set; }

        ISearchUrlWrapper _liveUrlWrapper;
    }
}