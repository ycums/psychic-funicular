using System;
using System.Drawing;
using System.Xml.Serialization;

namespace Pg01.Models
{
    [Serializable]
    public struct MenuItem
    {
        public string Key;
        public string Label;
        public string Type;
        public string Data;
        public string NextGroup;

        [XmlIgnore] public Color BackColor;

        [XmlElement("BackColor")]
        public string BackColorHtml
        {
            get { return ColorTranslator.ToHtml(BackColor); }
            set { BackColor = ColorTranslator.FromHtml(value); }
        }
    }
}