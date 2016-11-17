#region

using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using Pg01.Views.Behaviors.Util;

#endregion

namespace Pg01.Views.Behaviors
{
    public class NonActiveWindowBehavior : Behavior<Window>
    {
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
            NativeMethods.SetWindowLong(helper.Handle, NativeMethods.GWL_EXSTYLE,
                NativeMethods.GetWindowLong(helper.Handle, NativeMethods.GWL_EXSTYLE) | NativeMethods.WS_EX_NOACTIVATE);
        }
    }
}