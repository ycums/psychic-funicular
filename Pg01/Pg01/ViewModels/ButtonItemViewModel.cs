using System;
using Livet;
using Pg01.Models;

namespace Pg01.ViewModels
{
    public class ButtonItemViewModel : ViewModel
    {
        #region Initialize & Finalize

        public ButtonItemViewModel()
        {
            Key = "";
        }

        public ButtonItemViewModel(ButtonItem buttonItem)
        {
            Width = 40;
            Height = 24;

            X = buttonItem.X * Width;
            Y = buttonItem.Y * Height;
            Key = buttonItem.Key;
        }

        public void Initialize()
        {
        }


        #endregion

        #region Properties

        #region X変更通知プロパティ

        private double _X;

        public double X
        {
            get { return _X; }
            set
            {
                if (Math.Abs(_X - value) < ConstValues.TOLERANCE)
                    return;
                _X = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Y変更通知プロパティ

        private double _Y;

        public double Y
        {
            get { return _Y; }
            set
            {
                if (Math.Abs(_Y - value) < ConstValues.TOLERANCE)
                    return;
                _Y = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Key変更通知プロパティ

        private string _Key;

        public string Key
        {
            get { return _Key; }
            set
            {
                if (_Key == value)
                    return;
                _Key = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Width変更通知プロパティ

        private double _Width;

        public double Width
        {
            get { return _Width; }
            set
            {
                if (Math.Abs(_Width - value) < ConstValues.TOLERANCE)
                    return;
                _Width = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Height変更通知プロパティ

        private double _Height;

        public double Height
        {
            get { return _Height; }
            set
            {
                if (Math.Abs(_Height - value) < ConstValues.TOLERANCE)
                    return;
                _Height = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion
    }
}