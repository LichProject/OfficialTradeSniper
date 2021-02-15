using System.ComponentModel;
using TradeSniper.Common;

namespace TradeSniper.Settings
{
    public class GlobalSettings : JsonSettings
    {
        public GlobalSettings()
            : base(GetAbsoluteFilePath("GlobalSettings.json"))
        {
        }
        
        [Description("Значение POESESSID-Cookie")]
        public string PoeSessionId { get; set; }

        [Description("Коэффициент ожидания между запросами (1.01 = 101% от автоматической задержки)")]
        public double RequestsDelayFactor { get; set; } = 1.01;
    }
}