using ImGuiSniperHost.Common;

namespace ImGuiSniperHost.Settings
{
    public class GlobalSettings : JsonSettings
    {
        GlobalSettings()
            : base(GetAbsoluteFilePath("GlobalSettings.json"))
        {
        }

        public static readonly GlobalSettings Instance = new GlobalSettings();

        public string PoeSessionId
        {
            get => PoeSessionIdField;
            set => PoeSessionIdField = value ?? string.Empty;
        }

        public double RequestsDelayFactor
        {
            get => RequestDelayFactorField;
            set => RequestDelayFactorField = value;
        }

        #region ImGui fields

        public string PoeSessionIdField = string.Empty;
        public double RequestDelayFactorField = 1.01;

        #endregion
    }
}