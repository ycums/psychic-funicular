using System.Diagnostics;
using System.Windows;
using System.Windows.Interactivity;
using Pg01.Behaviors.Util;
using Pg01.Views;

namespace Pg01.Behaviors
{
    public class KeyboardHookBehavior : Behavior<Window>
    {
        private KeyboardHook _hook;

        protected override void OnAttached()
        {
            base.OnAttached();
            _hook = new KeyboardHook();
            _hook.KeyboardHooked += _hook_KeyboardHooked;
        }

        private void _hook_KeyboardHooked(object sender, KeyboardHookedEventArgs e)
        {
            Debug.WriteLine($"{e.KeyCode} {e.UpDown}");
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _hook.KeyboardHooked -= _hook_KeyboardHooked;
        }
    }
}