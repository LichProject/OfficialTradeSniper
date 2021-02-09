using System.Collections.Generic;
using TradeSniper.Common;
using TradeSniper.Models;

namespace TradeSniper.Settings
{
    public class SniperItems : JsonSettings
    {
        public SniperItems()
            : base(GetAbsoluteFilePath("SniperItems.json"))
        {
        }

        public List<SniperItem> SniperList { get; set; } = new List<SniperItem>();
    }
}