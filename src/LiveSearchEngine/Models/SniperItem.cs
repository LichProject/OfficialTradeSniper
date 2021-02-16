using System.ComponentModel;
using Newtonsoft.Json;

namespace LiveSearchEngine.Models
{
    public class SniperItem
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

        [Description("Хеш-код из поиска")]
        public string SearchHash { get; set; }

        [Description("Лига")]
        public string League { get; set; }

        [JsonIgnore]
        public LiveUrlWrapper LiveUrlWrapper => _liveUrlWrapper ?? (_liveUrlWrapper = new LiveUrlWrapper(this));

        LiveUrlWrapper _liveUrlWrapper;
    }
}