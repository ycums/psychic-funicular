using System;
using System.Collections.Generic;

namespace Pg01.Models
{
    [Serializable]
    public struct ApplicationGroup
    {
        public string Title;
        public string ExeName;
        public List<string> WindowPatterns;
        public List<MenuGroup> MenuGroups;

        public override string ToString()
        {
            return Title;
        }

        public void PropegateMgLabelChange(string before, string after)
        {
            foreach (var x in MenuGroups)
                for (var i = 0; i < x.MenuItems.Count; ++i)
                {
                    var y = x.MenuItems[i];
                    if (y.NextGroup == before)
                        y.NextGroup = after;
                    x.MenuItems[i] = y;
                }
        }
    }
}