using System;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public class ButtonItem
    {
        [XmlAttribute]
        public double X { get; set; }

        [XmlAttribute]
        public double Y { get; set; }

        [XmlAttribute]
        public string Key { get; set; }
    }
}