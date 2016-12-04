#region

using GcTest.AppUtil;

#endregion

namespace GcTest.Views
{
    /// <summary>
    ///     MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            NotifyDebugInfo.WriteLine("Create Window: MainWindow");
        }

        ~MainWindow()
        {
            NotifyDebugInfo.WriteLine("Destructor Window: MainWindow");
        }
    }
}