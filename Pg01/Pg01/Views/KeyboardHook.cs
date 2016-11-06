using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Pg01.Views
{

    #region "DllImportで使う引数の型の定義"

    public enum KeyboardUpDown
    {
        Down,
        Up
    }

    internal enum KeyboardMessage
    {
        KeyDown = 0x100,
        KeyUp = 0x101,
        SysKeyDown = 0x104,
        SysKeyUp = 0x105
    }

    internal struct KeyboardState
    {
        public Keys KeyCode;
        public int ScanCode;
        public KeyboardStateFlag Flag;
        public int Time;
        public IntPtr ExtraInfo;
    }

    internal struct KeyboardStateFlag
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

    [DefaultEvent("KeyboardHooked")]
    public class KeyboardHook : Component
    {
        private static readonly object EventKeyboardHooked = new object();

        public KeyboardHook()
        {
            DelegateKeyboardHook callback = CallNextHook;
            _keyHookDelegate = GCHandle.Alloc(callback);
            using (var process = Process.GetCurrentProcess())
            {
                using (var module = process.MainModule)
                {
                    var moduleHandler = GetModuleHandle(module.ModuleName);
                    _keyHook = SetWindowsHookEx(WH_KEYBOARD_LL, callback, moduleHandler, 0);
                }
            }
        }

        public event DelegateKeyboardHookedEvent KeyboardHooked
        {
            add { Events.AddHandler(EventKeyboardHooked, value); }
            remove { Events.RemoveHandler(EventKeyboardHooked, value); }
        }

        protected virtual void OnKeyboardHooked(
            KeyboardHookedEventArgs e
        )
        {
            var handler
                = Events[EventKeyboardHooked] as DelegateKeyboardHookedEvent;
            handler?.Invoke(this, e);
        }

        private int CallNextHook(
            int code,
            KeyboardMessage message,
            ref KeyboardState state
        )
        {
            if (code >= 0)
            {
                var e
                    = new KeyboardHookedEventArgs(
                        message, ref state);
                OnKeyboardHooked(e);
                if (e.Cancel)
                    return -1;
            }
            return CallNextHookEx(IntPtr.Zero, code, message, ref state);
        }

        protected override void Dispose(
            bool disposing
        )
        {
            if (_keyHookDelegate.IsAllocated)
            {
                UnhookWindowsHookEx(_keyHook);
                _keyHook = IntPtr.Zero;
                _keyHookDelegate.Free();
            }
            base.Dispose(disposing);
        }

        #region "DllImport"

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int hookType,
            DelegateKeyboardHook hookDelegate,
            IntPtr hInstance,
            uint threadId
        );

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int CallNextHookEx(
            IntPtr hook,
            int code,
            KeyboardMessage message,
            ref KeyboardState state
        );

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(
            IntPtr hook
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region "定数定義"

        // ReSharper disable UnusedMember.Local
        private const int WH_CALLWNDPROC = 4;
        private const int WH_KEYBOARD_LL = 13;
        private const int WA_ACTIVE = 1;
        private const int WA_CLICKACTIVE = 2;
        private const int WA_INACTIVE = 0;
        // ReSharper restore UnusedMember.Local

        #endregion

        #region "メンバ変数"

        private GCHandle _keyHookDelegate;
        private IntPtr _keyHook;

        #endregion

        #region "Delegates"

        public delegate void DelegateKeyboardHookedEvent(
            object sender,
            KeyboardHookedEventArgs e
        );

        private delegate int DelegateKeyboardHook(
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