using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Pg01.Views.Behaviors.Util
{
    public sealed class KeyboardHook:IDisposable
    {
        #region Events

        public event NativeMethods.DelegateKeyboardHookedEvent KeyboardHooked;

        #endregion

        #region Event Handlers

        private void OnKeyboardHooked(KeyboardHookedEventArgs e)
        {
            KeyboardHooked?.Invoke(this, e);
        }

        #endregion

        #region Methods

        private int CallNextHook(
            int code,
            NativeMethods.KeyboardMessage message,
            ref NativeMethods.KeyboardState state
        )
        {
            if (code >= 0)
            {
                var e = new KeyboardHookedEventArgs(message, ref state);
                OnKeyboardHooked(e);
                if (e.Cancel)
                    return -1;
            }
            return NativeMethods.CallNextHookEx(IntPtr.Zero, code, message, ref state);
        }

        #endregion

        #region Fields

        private GCHandle _keyHookDelegate;
        private IntPtr _keyHook;

        #endregion

        #region Initialize & Finalize

        public KeyboardHook()
        {
            NativeMethods.DelegateKeyboardHook callback = CallNextHook;
            _keyHookDelegate = GCHandle.Alloc(callback);
            using (var process = Process.GetCurrentProcess())
            {
                using (var module = process.MainModule)
                {
                    var moduleHandler = NativeMethods.GetModuleHandle(module.ModuleName);
                    _keyHook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, callback, moduleHandler, 0);
                }
            }
        }
        public void Dispose()
        {
            if (_keyHookDelegate.IsAllocated)
            {
                NativeMethods.UnhookWindowsHookEx(_keyHook);
                _keyHook = IntPtr.Zero;
                _keyHookDelegate.Free();
            }
        }

        #endregion
    }
}