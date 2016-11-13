using System;
using System.Windows;
using JetBrains.Annotations;
using Livet;
using Livet.EventListeners;
using Pg01.Models;

namespace Pg01.ViewModels
{
    public class MenuViewModel : ViewModel
    {
        #region Fields

        private readonly Model _model;
        [UsedImplicitly] private PropertyChangedEventListener _listener;

        #endregion

        #region Initialize & Finalize

        public MenuViewModel() : this(new Model())
        {
        }

        public MenuViewModel(Model model)
        {
            _model = model;
        }

        public void Initialize()
        {
            _listener = new PropertyChangedEventListener(_model)
            {
            };
        }

        #endregion

        #region Private Functions

        private void CorrectY()
        {
            Y = Math.Min(Math.Max(0, _model.Basic.WindowLocation.Y), SystemParameters.VirtualScreenHeight - Height);
        }

        private void CorrectX()
        {
            X = Math.Min(Math.Max(0, _model.Basic.WindowLocation.X), SystemParameters.VirtualScreenWidth - Width);
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
                CorrectX();
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
                CorrectY();
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
                CorrectX();
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
                CorrectY();
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Buttons変更通知プロパティ

        private ObservableSynchronizedCollection<ButtonItemViewModel> _Buttons;

        public ObservableSynchronizedCollection<ButtonItemViewModel> Buttons
        {
            get { return _Buttons; }
            set
            {
                if (_Buttons == value)
                    return;
                _Buttons = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ButtonsContainerWidth変更通知プロパティ

        private double _ButtonsContainerWidth;

        public double ButtonsContainerWidth
        {
            get { return _ButtonsContainerWidth; }
            set
            {
                if (Math.Abs(_ButtonsContainerWidth - value) < ConstValues.TOLERANCE)
                    return;
                _ButtonsContainerWidth = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ButtonsContainerHeight変更通知プロパティ

        private double _ButtonsContainerHeight;

        public double ButtonsContainerHeight
        {
            get { return _ButtonsContainerHeight; }
            set
            {
                if (Math.Abs(_ButtonsContainerHeight - value) < ConstValues.TOLERANCE)
                    return;
                _ButtonsContainerHeight = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #endregion
    }
}