using LiveSearchEngine.LiveSearch;
using TradeSniper.Settings;

namespace TradeSniper.Models
{
    public class CommandConfiguration
    {
        public GlobalSettings GlobalSettings { get; set; }
        public SniperSettings SniperSettings { get; set; }
        public ConsoleMenu Menu { get; set; }
        public LiveSearchWrapper LiveSearch { get; set; }
    }
}