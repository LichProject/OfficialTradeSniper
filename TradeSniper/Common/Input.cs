using System;
using System.Linq;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using TradeSniper.Enums;
using TradeSniper.Native;

namespace TradeSniper.Common
{
    public enum KeyState : short
    {
        Up = 0,
        Down = -127
    }

    /// <summary>
    /// Exposes a means to perform lower level input actions in the client.
    /// </summary>
    public static class Input
    {
        #region Base

        #endregion

        #region WM Storage

        #region Keyboard

        public const int WM_KEYUP = 0x101;
        public const int WM_KEYDOWN = 0x100;

        #endregion

        #region Mouse

        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;

        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_RBUTTONUP = 0x205;

        #endregion

        #endregion

        #region Methods

        #region Base

        public static bool IsKeyDown(VirtualKeyCode key)
        {
            return InputManager.InputDeviceState.IsKeyDown(key);
        }

        public static bool IsKeyDown(int key)
        {
            return IsKeyDown((VirtualKeyCode) key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return IsKeyDown((VirtualKeyCode) key);
        }

        #endregion

        #region Keyboard

        /// <summary>
        /// Clear all key states.
        /// </summary>
        public static void ClearAllKeyStates()
        {
            foreach (var key in (VirtualKeyCode[]) Enum.GetValues(typeof(VirtualKeyCode)))
            {
                if (IsKeyDown(key))
                {
                    InputManager.Keyboard.KeyUp(key);
                }
            }
        }

        public static void SimulateTextEntry(string text)
        {
            foreach (IntPtr value in text.Select(c => (IntPtr) c))
            {
                Interop.PostMessageW(Game.Proc.MainWindowHandle, (int) WindowsMessage.WM_CHAR, value, IntPtr.Zero);
            }
        }

        public static void InjectKey(Keys key)
        {
            Interop.SendMessageW(Game.Proc.MainWindowHandle, (int) WindowsMessage.WM_KEYDOWN, new IntPtr((int) key), IntPtr.Zero);
        }

        #endregion

        #endregion

        #region Cache

        static readonly InputSimulator InputManager = new InputSimulator();

        #endregion
    }
}