using System.Diagnostics;
using System.Linq;

namespace ImGuiSniperHost.Common
{
    public static class Game
    {
        public static Process Process
        {
            get
            {
                if (_process == null)
                {
                    var process = Process.GetProcessesByName("PathOfExile_x64").FirstOrDefault();
                    if (process == null)
                        return null;

                    _process = process;
                    _process.EnableRaisingEvents = true;
                    _process.Exited += (_, __) => _process = null;
                }

                return _process;
            }
        }

        static Process _process;
    }
}