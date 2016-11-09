using Pg01.Behaviors.Util;

namespace Pg01.Models.Util
{
    public interface ISendKeyCode
    {
        void SendKey(string data, NativeMethods.KeyboardUpDown kud);
        void SendWait(string data);
    }
}