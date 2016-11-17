#region

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#endregion

namespace Pg01.Models
{
    [Serializable]
    public class ApplicationGroup
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public MatchingRoule MatchingRoule { get; set; }

        [XmlArrayItem("Bank")]
        public List<Bank> Banks { get; set; }

        [XmlArrayItem("Menu")]
        public List<Menu> Menus { get; set; }
    }
}