#region

using System;
using System.Text;
using Pg01.Views.Behaviors.Util;

#endregion

namespace Pg01.Models.Util
{
    public class ForegroundWidowInfo
    {
        #region Wrapper Functions

        public static string GetWindowText()
        {
            var hWnd = NativeMethods.GetForegroundWindow();

            if (NativeMethods.IsWindowVisible(hWnd))
            {
                var winLen = NativeMethods.GetWindowTextLength(hWnd);

                var sb = new StringBuilder(winLen + 1);
                NativeMethods.GetWindowText(hWnd, sb, sb.Capacity);

                return sb.ToString();
            }
            return "";
        }

        public static string GetWindowClass()
        {
            var hWnd = NativeMethods.GetForegroundWindow();

            if (NativeMethods.IsWindowVisible(hWnd))
            {
                var csb = new StringBuilder(256);
                NativeMethods.GetClassName(hWnd, csb, csb.Capacity);

                return csb.ToString();
            }
            return "";
        }

        public static string GetParentWindowClass()
        {
            var hWnd = NativeMethods.GetForegroundWindow();

            if (NativeMethods.IsWindowVisible(hWnd))
            {
                var hWndParent = NativeMethods.GetParent(hWnd);
                var builder = new StringBuilder(256);
                if (hWndParent != (IntPtr) 0)
                {
                    NativeMethods.GetClassName(hWndParent, builder, builder.Capacity);
                }
                else
                {
                    builder.AppendLine("なし");
                }

                return builder.ToString();
            }
            return "";
        }

        public static string GetExeName()
        {
            var hWnd = NativeMethods.GetForegroundWindow();
            uint lpdwProcessId;
            NativeMethods.GetWindowThreadProcessId(hWnd, out lpdwProcessId);

            var hProcess = NativeMethods.OpenProcess(0x0410, false, lpdwProcessId);

            var text = new StringBuilder(1000);
            NativeMethods.GetModuleFileNameEx(hProcess, IntPtr.Zero, text, text.Capacity);

            NativeMethods.CloseHandle(hProcess);

            return text.ToString();
        }

        #endregion
    }
}