#region

using System;
using System.Windows.Media;
using System.Xml.Serialization;

#endregion

namespace Pg01.Models
{
    [Serializable]
    public class Entry
    {
        private Brush _background;

        [XmlAttribute]
        public string Trigger { get; set; }

        [XmlAttribute]
        public string LabelText { get; set; }

        [XmlIgnore]
        public Brush Background
        {
            get { return _background ?? Util.Util.DefaultBrush; }
            set { _background = value; }
        }

        [XmlAttribute("BackColor")]
        public string LabelColorAsString
        {
            get { return Util.Util.ConvertToString(Background); }
            set
            {
                Background = string.IsNullOrEmpty(value)
                    ? Util.Util.DefaultBrush
                    : Util.Util.ConvertFromString<Brush>(value);
            }
        }

        [XmlIgnore]
        public Brush Foreground
        {
            get
            {
                var b = Background as SolidColorBrush;
                return b != null ? new SolidColorBrush(Util.Util.GetDisplayForeColor(b.Color)) : Brushes.Black;
            }
        }

        [XmlElement]
        public ActionItem ActionItem { get; set; }
    }
}