using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public class Bank
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Entry")]
        public List<Entry> Entries { get; set; }
    }
}