#region

using System.Windows;
using GcTest.AppUtil;

#endregion

namespace GcTest.Views

{
    /// <summary>
    ///     DetailWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DetailWindow

    {
        public DetailWindow()
        {
            InitializeComponent();

            NotifyDebugInfo.WriteLine("Create Window: DetailWindow");
        }


        ~DetailWindow()

        {
            NotifyDebugInfo.WriteLine("Destructor Window: DetailWindow");
        }
    }
}