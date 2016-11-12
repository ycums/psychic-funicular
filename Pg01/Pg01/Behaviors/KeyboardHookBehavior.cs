using System.Windows;
using System.Windows.Interactivity;
using Pg01.Behaviors.Util;

namespace Pg01.Behaviors
{
    public class KeyboardHookBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event",
            typeof(KeyboardHookedEventArgs),
            typeof(KeyboardHookBehavior), new PropertyMetadata(default(KeyboardHookedEventArgs)));

        private KeyboardHook _hook;

        public KeyboardHookedEventArgs Event
        {
            get { return (KeyboardHookedEventArgs) GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _hook = new KeyboardHook();
            _hook.KeyboardHooked += _hook_KeyboardHooked;
        }

        private void _hook_KeyboardHooked(object sender, KeyboardHookedEventArgs e)
        {
            Event = e;
        }

        protected override void OnDetaching()
        {
            _hook.KeyboardHooked -= _hook_KeyboardHooked;
            base.OnDetaching();
        }
    }
}