using System.Threading.Tasks;
using ImGuiSniperHost.Common;
using ImGuiSniperHost.Controllers;

namespace ImGuiSniperHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var imGui = new ImGuiOverlay(new MainController());
            await imGui.Run();
        }
    }
}