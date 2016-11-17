using Pg01.Models.Util;
using Pg01.Views.Behaviors.Util;

namespace Pg01Tests.ViewModels
{
    public class DummySendKeyCode : ISendKeyCode
    {
        public void SendKey(string str, NativeMethods.KeyboardUpDown keyboardUpDown)
        {
        }

        public void SendWait(string p)
        {
        }
    }
}