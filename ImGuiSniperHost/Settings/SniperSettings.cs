using System.Collections.Generic;
using ImGuiSniperHost.Common;
using LiveSearchEngine.Models;

namespace ImGuiSniperHost.Settings
{
    public class SniperSettings : JsonSettings
    {
        SniperSettings()
            : base(GetAbsoluteFilePath("SniperSettings.json"))
        {
        }

        public static readonly SniperSettings Instance = new SniperSettings();

        public List<SniperItem> SniperItems { get; set; } = new List<SniperItem>();

        public void AddSniper(SniperItem sniperItem) => SniperItems.Add(sniperItem);
        public void RemoveSniper(SniperItem sniperItem) => SniperItems.Remove(sniperItem);
    }
}