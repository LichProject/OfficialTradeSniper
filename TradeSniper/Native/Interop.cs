using System;
using System.Runtime.InteropServices;

namespace TradeSniper.Native
{
    public static class Interop
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);
    }
}