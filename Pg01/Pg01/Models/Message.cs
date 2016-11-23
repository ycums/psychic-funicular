using System.Windows;

namespace Pg01.Models
{
    public class Message
    {
        public Message(string caption, MessageBoxImage image, string text)
        {
            Caption = caption;
            Image = image;
            Text = text;
        }

        public string Text { get; set; }
        public string Caption { get; set; }
        public MessageBoxImage Image { get; set; }
    }
}