#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

// ReSharper disable UnusedMember.Global

namespace Pg01.Views.Behaviors.Util
{
    public static class NativeMethods
    {
        #region "DllImportで使う引数の型の定義"

        public enum KeyboardUpDown
        {
            Down,
            Up,
            None
        }

        public enum KeyboardMessage
        {
            KeyDown = 0x100,
            KeyUp = 0x101,
            SysKeyDown = 0x104,
            SysKeyUp = 0x105
        }

        [SuppressMessage("ReSharper", "UnassignedField.Global")]
        public struct KeyboardState
        {
            public Keys KeyCode;
            public int ScanCode;
            public KeyboardStateFlag Flag;
            public int Time;
            public IntPtr ExtraInfo;
        }

        public struct KeyboardStateFlag
        {
            private int _flag;

            private bool IsFlagging(int value)
            {
                return (_flag & value) != 0;
            }

            private void Flag(bool value, int digit)
            {
                _flag = value ? _flag | digit : _flag & ~digit;
            }

            public bool IsExtended
            {
                get { return IsFlagging(0x01); }
                set { Flag(value, 0x01); }
            }

            public bool IsInjected
            {
                get { return IsFlagging(0x10); }
                set { Flag(value, 0x10); }
            }

            public bool AltDown
            {
                get { return IsFlagging(0x20); }
                set { Flag(value, 0x20); }
            }

            public bool IsUp
            {
                get { return IsFlagging(0x80); }
                set { Flag(value, 0x80); }
            }
        }

        #endregion

        #region "DllImport"

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(
            int hookType,
            DelegateKeyboardHook hookDelegate,
            IntPtr hInstance,
            uint threadId
        );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int CallNextHookEx(
            IntPtr hook,
            int code,
            KeyboardMessage message,
            ref KeyboardState state
        );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(
            IntPtr hook
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        #endregion

        #region "定数定義"

        // ReSharper disable UnusedMember.Local
        private const int WH_CALLWNDPROC = 4;
        public const int WH_KEYBOARD_LL = 13;
        private const int WA_ACTIVE = 1;
        private const int WA_CLICKACTIVE = 2;
        private const int WA_INACTIVE = 0;
        // ReSharper restore UnusedMember.Local

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        #endregion

        #region "Delegates"

        public delegate void DelegateKeyboardHookedEvent(
            object sender,
            KeyboardHookedEventArgs e
        );

        public delegate int DelegateKeyboardHook(
            int code,
            KeyboardMessage message,
            ref KeyboardState state
        );

        // ReSharper disable once UnusedMember.Local

        private delegate IntPtr DelegateWindowHook(
            int id,
            IntPtr wParam,
            IntPtr lParam
        );

        #endregion
    }
}