using WindowsInput;
using WindowsInput.Native;

namespace ImGuiSniperHost.Common
{
    public static class Input
    {
        public static void SendKey(VirtualKeyCode key)
        {
            InputSimulator.Keyboard.KeyPress(key);
        }

        public static void SendText(string text)
        {
            InputSimulator.Keyboard.TextEntry(text);
        }
        
        static readonly InputSimulator InputSimulator = new InputSimulator();
    }
}