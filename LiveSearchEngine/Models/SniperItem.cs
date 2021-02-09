using System.ComponentModel;
using Newtonsoft.Json;

namespace LiveSearchEngine.Models
{
    public class SniperItem
    {
        public SniperItem(string description, string searchHash, string league, int minStock = 0)
        {
            Description = description;
            SearchHash = searchHash;
            MinimalStock = minStock;
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

        [Description("Минимальный сток")]
        public int MinimalStock { get; set; }

        [JsonIgnore]
        public LiveUrlWrapper LiveUrlWrapper => _liveUrlWrapper ?? (_liveUrlWrapper = new LiveUrlWrapper(SearchHash, League));

        public bool CompareStock(int stock) => stock >= MinimalStock;

        LiveUrlWrapper _liveUrlWrapper;
    }
}