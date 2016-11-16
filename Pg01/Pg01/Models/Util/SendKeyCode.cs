#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Pg01.Views.Behaviors.Util;

#endregion

namespace Pg01.Models.Util
{
    public class SendKeyCode : ISendKeyCode
    {
        #region Tables

        private readonly Dictionary<string, Keys> _dicKeys = new Dictionary<string, Keys>
        {
            {" ", Keys.Space},
            {"{UP}", Keys.Up},
            {"{DOWN}", Keys.Down},
            {"{LEFT}", Keys.Left},
            {"{RIGHT}", Keys.Right},
            {"{ENTER}", Keys.Enter},
            {"{F16}", Keys.F16},
            {"{F17}", Keys.F17},
            {"{F18}", Keys.F18},
            {"{F19}", Keys.F19},
            {"{F20}", Keys.F20},
            {"{F21}", Keys.F21},
            {"{F22}", Keys.F22},
            {"{F23}", Keys.F23},
            {"{F24}", Keys.F24}
        };

        private readonly Dictionary<string, byte> _dicVk = new Dictionary<string, byte>
        {
            {"{ALT}", VK_MENU}
        };

        private readonly List<string> _sendWaitExtentionKeyList = new List<string>
        {
            "{F16}",
            "{F17}",
            "{F18}",
            "{F19}",
            "{F20}",
            "{F21}",
            "{F22}",
            "{F23}",
            "{F24}"
        };

        #endregion

        #region Const Values

        // ReSharper disable UnusedMember.Local
        private const byte VK_SHIFT = 0x10;
        private const byte VK_CONTROL = 0x11;
        private const byte VK_MENU = 0x12;
        private const byte VK_LWIN = 0x5B;
        private const byte VK_RWIN = 0x5C;
        private const byte VK_APPS = 0x5D;
        private const byte KEYEVENTF_KEYDOWN = 0x00;
        private const byte KEYEVENTF_KEYUP = 0x02;
        // ReSharper restore UnusedMember.Local

        #endregion

        #region Public Functions

        public static string Conv(Keys k)
        {
            return k.Between(Keys.D0, Keys.D9) ? k.ToString().Substring(1) : k.ToString();
        }

        private byte Conv(string str)
        {
            var b = (byte) Keys.Space;
            if (_dicKeys.ContainsKey(str))
            {
                b = (byte) _dicKeys[str];
            }
            else if (_dicVk.ContainsKey(str))
            {
                b = _dicVk[str];
            }
            else if (0 < str.Length)
            {
                var c = str.ToCharArray()[0];
                b = (byte) (Keys) c;
            }
            return b;
        }

        public void SendKey(string str, NativeMethods.KeyboardUpDown keyboardUpDown)
        {
            var p = Conv(str);
            switch (keyboardUpDown)
            {
                case NativeMethods.KeyboardUpDown.Down:
                    NativeMethods.keybd_event(p, 0, KEYEVENTF_KEYDOWN, (UIntPtr) 0);
                    break;
                case NativeMethods.KeyboardUpDown.Up:
                    NativeMethods.keybd_event(p, 0, KEYEVENTF_KEYUP, (UIntPtr) 0);
                    break;
            }
        }

        public void SendWait(string p)
        {
            if (_sendWaitExtentionKeyList.Contains(p))
            {
                SendKey(p, NativeMethods.KeyboardUpDown.Down);
                SendKey(p, NativeMethods.KeyboardUpDown.Up);
            }
            else
            {
                try
                {
                    SendKeys.SendWait(p);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        #endregion
    }
}