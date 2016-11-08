using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public class ApplicationGroup
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public MatchingRoule MatchingRoule { get; set; }

        [XmlElement]
        public List<Bank> Banks { get; set; }
    }
}