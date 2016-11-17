using System;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public class ActionItem
    {
        [XmlAttribute]
        public ActionType ActionType { get; set; }

        [XmlAttribute]
        public string ActionValue { get; set; }

        [XmlAttribute]
        public string NextBank { get; set; }
    }
}