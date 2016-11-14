namespace Pg01.Models
{
    public class WindowInfo
    {
        public WindowInfo(string exeName, string windowName)
        {
            ExeName = exeName;
            WindowName = windowName;
        }

        public string WindowName { get; set; }

        public string ExeName { get; set; }
    }
}