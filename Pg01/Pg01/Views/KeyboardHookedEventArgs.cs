using System.ComponentModel;
using System.Windows.Forms;

namespace Pg01.Views
{
    public class KeyboardHookedEventArgs : CancelEventArgs
    {
        private readonly NativeMethods.KeyboardMessage _message;
        private NativeMethods.KeyboardState _state;

        internal KeyboardHookedEventArgs(
            NativeMethods.KeyboardMessage message,
            ref NativeMethods.KeyboardState state
            )
        {
            _message = message;
            _state = state;
        }

        public NativeMethods.KeyboardUpDown UpDown => _message == NativeMethods.KeyboardMessage.KeyDown
                                        || _message == NativeMethods.KeyboardMessage.SysKeyDown
            ? NativeMethods.KeyboardUpDown.Down
            : NativeMethods.KeyboardUpDown.Up;

        public Keys KeyCode => _state.KeyCode;

        public int ScanCode => _state.ScanCode;

        public bool IsExtendedKey => _state.Flag.IsExtended;

        public bool AltDown => _state.Flag.AltDown;
    }
}