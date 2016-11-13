using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace Pg01.Views.Behaviors
{
    public class NonActiveWindowBehavior : Behavior<Window>
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;

        private const int WS_EX_NOACTIVATE = 0x08000000;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SourceInitialized -= AssociatedObject_SourceInitialized;
            base.OnDetaching();
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            var helper = new WindowInteropHelper(AssociatedObject);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }
    }
}