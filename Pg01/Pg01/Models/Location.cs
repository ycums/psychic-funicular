using System;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public class Location
    {
        [XmlAttribute]
        public int X { get; set; }

        [XmlAttribute]
        public int Y { get; set; }
    }
}