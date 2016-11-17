namespace Pg01.Models
{
    public class WindowInfo
    {
        public WindowInfo(string exeName, string windowText)
        {
            ExeName = exeName;
            WindowText = windowText;
        }

        public string WindowText { get; set; }

        public string ExeName { get; set; }
    }
}