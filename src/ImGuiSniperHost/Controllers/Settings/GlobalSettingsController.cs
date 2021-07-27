using ImGuiNET;
using ImGuiSniperHost.Settings;

namespace ImGuiSniperHost.Controllers.Settings
{
    public class GlobalSettingsController : BaseSettingsController
    {
        GlobalSettings Settings => GlobalSettings.Instance;

        #region Overrides of BaseSettingsController

        public override void Draw()
        {
            ImGui.InputText(nameof(Settings.PoeSessionId), ref Settings.PoeSessionIdField, 256);
            ImGui.InputDouble(nameof(Settings.RequestsDelayFactor), ref Settings.RequestDelayFactorField, 1);

            DrawSaveButton(Settings, "globalsettings");
        }

        #endregion
    }
}