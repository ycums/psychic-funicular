using System.ComponentModel;
using System.Windows.Forms;

namespace Pg01.Views
{
    public class KeyboardHookedEventArgs : CancelEventArgs
    {
        private readonly KeyboardMessage _message;
        private KeyboardState _state;

        internal KeyboardHookedEventArgs(
            KeyboardMessage message,
            ref KeyboardState state
            )
        {
            _message = message;
            _state = state;
        }

        public KeyboardUpDown UpDown => _message == KeyboardMessage.KeyDown
                                        || _message == KeyboardMessage.SysKeyDown
            ? KeyboardUpDown.Down
            : KeyboardUpDown.Up;

        public Keys KeyCode => _state.KeyCode;

        public int ScanCode => _state.ScanCode;

        public bool IsExtendedKey => _state.Flag.IsExtended;

        public bool AltDown => _state.Flag.AltDown;
    }
}