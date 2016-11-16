using Pg01.Views.Behaviors.Util;

namespace Pg01.Models.Util
{
    public interface ISendKeyCode
    {
        void SendKey(string str, NativeMethods.KeyboardUpDown keyboardUpDown);
        void SendWait(string p);
    }
}