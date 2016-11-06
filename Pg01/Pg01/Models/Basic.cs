using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Serialization;
using Livet;

namespace Pg01.Models
{
    [Serializable]
    public class Basic : NotificationObject
    {
        [XmlElement]
        public string Title { get; set; }

        [XmlArrayItem("Button")]
        public List<ButtonItem> Buttons { get; set; }

        [XmlElement]
        public Location WindowLocation { get; set; }
    }
}