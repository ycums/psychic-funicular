using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public class MatchingRoule
    {
        [XmlElement]
        public string ExeName { get; set; }

        [XmlArrayItem("Pattern")]
        public List<string> WindowTitlePatterns { get; set; }
    }
}