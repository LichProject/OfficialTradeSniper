using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using ImGuiSniperHost.Common;
using LiveSearchEngine.Enums;

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

                if (ImGui.Button("ImGui Demo", GlobalStyle.LargeButtonSize))
                    DemoWindowActive = true;

                if (DemoWindowActive)
                    ImGui.ShowDemoWindow();

                ImGui.Separator();

                if (SniperWindowActive)
                    _sniperController.Draw();

                if (SettingsWindowActive)
                    _settingsController.Draw();

                ImGui.NewLine();

                ImGui.Text("Logs");
                ImGui.SameLine();
                
                if (ImGui.SmallButton("Clear"))
                    Logger.Clear();

                ImGui.BeginChild("logs_child_window", GlobalStyle.LoggerChildSize, true, ImGuiWindowFlags.AlwaysAutoResize);

                foreach (var (level, message) in Logger.GetList().TakeLast(100).ToList())
                {
                    var color = level switch
                    {
                        LogLevel.Warn => Color.Yellow,
                        LogLevel.Error => Color.Red,
                        _ => Color.White,
                    };

                    ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + 480);
                    ImGui.TextColored(color.ToImGuiVec4(), $"[{level}] {message}");
                    ImGui.PopTextWrapPos();

                    // auto-scroll
                    if (Math.Abs(ImGui.GetScrollY() - ImGui.GetScrollMaxY()) < 0.1f)
                        ImGui.SetScrollHereY(0);
                }

                ImGui.EndChild();
            }
            else
            {
                SniperWindowActive = false;
                SettingsWindowActive = false;

                ImGui.Text("There's no game process found.");
            }

            ImGui.End();
        }

        static ImGuiLogger Logger => ImGuiLogger.Instance;

        readonly SniperController _sniperController = new SniperController();
        readonly SettingsController _settingsController = new SettingsController();
    }
}