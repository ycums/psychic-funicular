#region

using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

#endregion

namespace GcTest.Behavior
{
    public class CommandExecuteAction : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command", typeof(ICommand), typeof(CommandExecuteAction),
                new PropertyMetadata());

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            Command?.Execute(parameter);
        }
    }
}