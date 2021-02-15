using ImGuiNET;
using ImGuiSniperHost.Common;

namespace ImGuiSniperHost.Controllers
{
    public class MainController : IDrawController
    {
        public MainController()
        {
            SniperWindowActive = true;
        }

        #region States

        bool SniperWindowActive
        {
            get => States[nameof(SniperWindowActive)];
            set => States[nameof(SniperWindowActive)] = value;
        }

        bool SettingsWindowActive
        {
            get => States[nameof(SettingsWindowActive)];
            set => States[nameof(SettingsWindowActive)] = value;
        }

        bool DemoWindowActive
        {
            get => States[nameof(DemoWindowActive)];
            set => States[nameof(DemoWindowActive)] = value;
        }

        #endregion

        public static StatesCacheContainer States { get; } = new StatesCacheContainer();

        public void Draw()
        {
            ImGui.Begin(GlobalStyle.WindowTitle, ref ImGuiOverlay.IsRunning, GlobalStyle.DefaultWindowFlags);

            if (Game.Process != null)
            {
                if (ImGui.Button("Sniper", GlobalStyle.LargeButtonSize))
                {
                    SniperWindowActive = true;
                    SettingsWindowActive = false;
                }

                ImGui.SameLine();

                if (ImGui.Button("Settings", GlobalStyle.LargeButtonSize))
                {
                    SniperWindowActive = false;
                    SettingsWindowActive = true;
                }

                ImGui.SameLine();

                if (ImGui.Button("Demo", GlobalStyle.LargeButtonSize))
                    DemoWindowActive = true;

                if (DemoWindowActive)
                    ImGui.ShowDemoWindow();

                ImGui.Separator();

                if (SniperWindowActive)
                    _sniperController.Draw();

                if (SettingsWindowActive)
                    _settingsController.Draw();
            }
            else
            {
                SniperWindowActive = false;
                SettingsWindowActive = false;

                ImGui.Text("There's no game process found.");
            }

            ImGui.End();
        }

        readonly SniperController _sniperController = new SniperController();
        readonly SettingsController _settingsController = new SettingsController();
    }
}