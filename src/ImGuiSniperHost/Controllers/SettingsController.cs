using ImGuiNET;
using ImGuiSniperHost.Controllers.Settings;

namespace ImGuiSniperHost.Controllers
{
    public class SettingsController : IDrawController
    {
        #region States

        bool GlobalSettingsActive
        {
            get => MainController.States[nameof(GlobalSettingsActive)];
            set => MainController.States[nameof(GlobalSettingsActive)] = value;
        }

        bool SniperSettingsActive
        {
            get => MainController.States[nameof(SniperSettingsActive)];
            set => MainController.States[nameof(SniperSettingsActive)] = value;
        }

        #endregion

        #region Overrides of BaseController

        public void Draw()
        {
            if (ImGui.Button("Global settings", GlobalStyle.LargeButtonSize))
            {
                GlobalSettingsActive = true;
                SniperSettingsActive = false;
            }

            ImGui.SameLine();

            if (ImGui.Button("Sniper settings", GlobalStyle.LargeButtonSize))
            {
                GlobalSettingsActive = false;
                SniperSettingsActive = true;
            }

            if (GlobalSettingsActive)
                _globalSettingsController.Draw();

            if (SniperSettingsActive)
                _sniperSettingsController.Draw();
        }

        #endregion

        readonly GlobalSettingsController _globalSettingsController = new GlobalSettingsController();
        readonly SniperSettingsController _sniperSettingsController = new SniperSettingsController();
    }
}