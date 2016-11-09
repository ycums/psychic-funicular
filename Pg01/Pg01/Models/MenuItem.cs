using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public class MenuItem
    {
        [XmlAttribute]
        public double X { get; set; }

        [XmlAttribute]
        public double Y { get; set; }

        [XmlAttribute]
        public string LabelText { get; set; }

        [XmlIgnore]
        public Color BackColor { get; set; }

        [XmlAttribute("BackColor")]
        public string LabelColorAsString
        {
            get { return Util.Util.ConvertToString(BackColor); }
            set { BackColor = Util.Util.ConvertFromString<Color>(value); }
        }

        [XmlElement]
        public ActionItem Action { get; set; }
    }
}