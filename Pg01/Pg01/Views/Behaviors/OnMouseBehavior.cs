#region

using System;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interactivity;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

#endregion

namespace Pg01.Views.Behaviors
{
    public class OnMouseBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty OnMouseProperty =
            DependencyProperty.Register("OnMouse", typeof(bool),
                typeof(OnMouseBehavior), new PropertyMetadata(default(bool)));

        private readonly DispatcherTimer _timer;

        public OnMouseBehavior()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
        }

        public bool OnMouse
        {
            get { return (bool) GetValue(OnMouseProperty); }
            set { SetValue(OnMouseProperty, value); }
        }

        private void _timer_Elapsed(object sender, EventArgs eventArgs)
        {
            var w = AssociatedObject;
            var rect = new Rect(w.Left, w.Top, w.Width, w.Height);
            var pos = Cursor.Position;
            OnMouse = rect.Contains(new Point(pos.X,pos.Y));
        }

        protected override void OnAttached()
        {
            _timer.Tick += _timer_Elapsed;
            _timer.Start();
            base.OnAttached();
        }


        protected override void OnDetaching()
        {
            _timer.Tick -= _timer_Elapsed;
            _timer.Stop();
            base.OnDetaching();
        }
    }
}