using System.Numerics;
using ImGuiNET;

namespace ImGuiSniperHost
{
    public static class GlobalStyle
    {
        #region Window

        public const ImGuiWindowFlags DefaultWindowFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize;
        public const string WindowTitle = "Sniper Engine (c) LichProject";

        #endregion

        #region Buttons

        public static readonly Vector2 LargeButtonSize = new Vector2(150, 20);

        #endregion
    }
}