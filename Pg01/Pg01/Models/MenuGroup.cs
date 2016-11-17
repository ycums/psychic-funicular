using System;
using System.Collections.Generic;

namespace Pg01.Models
{
    [Serializable]
    public struct MenuGroup
    {
        public string Label;
        public int Timeout;
        public List<MenuItem> MenuItems;

        public override string ToString()
        {
            return Label.Length == 0 ? "(default)" : Label;
        }
    }
}