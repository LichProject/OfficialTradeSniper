using System;
using System.Diagnostics;
using System.Linq;

namespace TradeSniper.Common
{
    public class Game
    {
        public string DefaultProcessName = "PathOfExile_x64.exe";
        
        public int LookupProcessId()
        {
            var process = Process.GetProcessesByName(DefaultProcessName).FirstOrDefault();
            if (process == null)
                return 0;

            return process.Id;
        }

        public void SetForegroundWindow()
        {
            throw new NotImplementedException();
        }

        public void SendChatFmt(string text, object args = null)
        {
            SendChat(string.Format(text, args));
        }

        public void SendChat(string text)
        {
            throw new NotImplementedException();
        }
    }
}