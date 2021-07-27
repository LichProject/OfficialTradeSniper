using System.Threading.Tasks;
using ClickableTransparentOverlay;
using ImGuiSniperHost.Controllers;

namespace ImGuiSniperHost.Common
{
    public class ImGuiOverlay : Overlay
    {
        public ImGuiOverlay(IDrawController mainGui)
        {
            MainGui = mainGui;
        }

        public static IDrawController MainGui { get; private set; }
        public static bool IsRunning = true;


        #region Overrides of Overlay

        protected override Task Render()
        {
            MainGui.Draw();

            if (!IsRunning)
                Close();

            return Task.CompletedTask;
        }

        #endregion
    }
}