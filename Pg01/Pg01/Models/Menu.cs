using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public class Menu
    {
        [XmlAttribute] public string Name;

        [XmlElement]
        public List<MenuItem> MenuItem { get; set; }
    }
}