#region

using System.Collections.Generic;
using Pg01.Models.Util;
using Pg01.Views.Behaviors.Util;

#endregion

namespace Pg01Tests.ViewModels
{
    public class DummySendKeyCode : ISendKeyCode
    {
        public enum EventType
        {
            None,
            SendKey,
            SendWait
        }

        public DummySendKeyCode()
        {
            EventLog = new List<Event>();
        }

        public List<Event> EventLog { get; set; }

        public void SendKey(string str,
            NativeMethods.KeyboardUpDown keyboardUpDown)
        {
            EventLog.Add(new Event(EventType.SendKey, str, keyboardUpDown));
        }

        public void SendWait(string p)
        {
            EventLog.Add(new Event(EventType.SendWait, p,
                NativeMethods.KeyboardUpDown.None));
        }

        public class Event
        {
            public NativeMethods.KeyboardUpDown Kud;
            public EventType Type;
            public string Value;

            public Event(EventType type, string value,
                NativeMethods.KeyboardUpDown kud)
            {
                Kud = kud;
                Type = type;
                Value = value;
            }
        }
    }
}