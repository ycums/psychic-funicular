using System.Windows.Media;
using Livet.Commands;
using Pg01.Models;

namespace Pg01.ViewModels
{
    public interface IButtonItemViewModel
    {
        #region Properties

        ActionItem ActionItem { get; set; }
        Brush Background { get; set; }
        bool Enabled { get; set; }
        double Height { get; set; }
        string LabelText { get; set; }
        double Width { get; set; }
        double X { get; set; }
        double Y { get; set; }

        #endregion

        #region Commands

        ViewModelCommand ButtonCommand { get; }
        void Button();

        #endregion
    }
}