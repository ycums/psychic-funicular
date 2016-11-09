using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using Livet;

namespace Pg01.Models
{
    [Serializable]
    public class ButtonItem : NotificationObject
    {
        [XmlAttribute]
        public double X { get; set; }

        [XmlAttribute]
        public double Y { get; set; }

        [XmlAttribute]
        public string Key { get; set; }

        [XmlIgnore]
        public string LabeText { get; set; }

        [XmlIgnore]
        public Color BackColor { get; set; }

        [XmlIgnore]
        public bool Enabled { get; set; }
    }
}