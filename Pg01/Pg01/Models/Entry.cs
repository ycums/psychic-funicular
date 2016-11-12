using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public class Entry
    {
        [XmlAttribute]
        public string Trigger { get; set; }

        [XmlAttribute]
        public string LabelText { get; set; }

        [XmlIgnore]
        public Brush Background { get; set; }

        [XmlAttribute("BackColor")]
        public string LabelColorAsString
        {
            get { return Util.Util.ConvertToString(Background); }
            set { Background = Util.Util.ConvertFromString<Brush>(value); }
        }

        [XmlElement]
        public ActionItem ActionItem { get; set; }
    }
}