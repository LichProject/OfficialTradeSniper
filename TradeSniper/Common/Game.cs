using System.Diagnostics;
using System.Linq;

namespace TradeSniper.Common
{
    public static class Game
    {
        public static Process Proc
        {
            get
            {
                if (_process == null)
                {
                    var process = Process.GetProcessesByName("PathOfExile_x64").FirstOrDefault();
                    if (process == null)
                        return null;

                    _process = process;
                }

                return _process;
            }
        }

        static Process _process;
    }
}