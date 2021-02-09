using System.Collections.Generic;
using LiveSearchEngine.Models;
using TradeSniper.Common;

namespace TradeSniper.Settings
{
    public class SniperSettings : JsonSettings
    {
        public SniperSettings()
            : base(GetAbsoluteFilePath("SniperSettings.json"))
        {
        }

        public List<SniperItem> SniperItems { get; set; } = new List<SniperItem>();

        public void AddSniper(SniperItem sniperItem) => SniperItems.Add(sniperItem);
        public void RemoveSniper(SniperItem sniperItem) => SniperItems.Remove(sniperItem);
    }
}