#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Livet;
using Livet.EventListeners;
using Pg01.Models;
using Pg01Util;

#endregion

namespace Pg01.ViewModels
{
    public class MenuViewModel : ViewModel
    {
        #region Fields

        private readonly Model _model;

        #endregion

        #region Private Functions

        public void UpdateMenu(Model model)
        {
            var items = model.Menu.MenuItem;
            ButtonsOrigin =
                new System.Windows.Point(
                    -items.Max(x => Math.Abs(x.X)),
                    -items.Max(x => Math.Abs(x.Y)));
            ButtonsContainerWidth =
                (-ButtonsOrigin.X*2 + 1)*ConstValues.ButtonWidth;
            ButtonsContainerHeight =
                (-ButtonsOrigin.Y*2 + 1)*
                ConstValues.ButtonHeight;
            Buttons =
                new ObservableSynchronizedCollection<MenuItemViewModel>(
                    model.Menu.MenuItem.Select(
                            x =>
                                new MenuItemViewModel(
                                    model, x, ButtonsOrigin))
                        .ToArray());
            Debug.WriteLine($"UpdateMenu: {Buttons.Count}");
        }

        public void IsMenuVisibleChanged(bool visible, Point pos)
        {
            if (visible)
            {
                X = pos.X - ButtonsContainerWidth/2;
                Y = pos.Y - ButtonsContainerHeight/2;
            }
            IsMenuVisible = visible;
        }

        #endregion

        #region Initialize & Finalize

        public MenuViewModel(Model model)
        {
            ViewModelManager.AddEntryViewModel(this);
            ObjectCountManager.CountUp(GetType());

            _model = model;
            _X = -99999; //ちらつき防止

            var listener = new PropertyChangedEventListener(_model)
            {
                {
                    () => _model.Menu,
                    (s, a) => UpdateMenu(_model)
                },
                {
                    () => _model.IsMenuVisible,
                    (s, a) =>
                        IsMenuVisibleChanged(
                            _model.IsMenuVisible, Cursor.Position)
                }
            };
            CompositeDisposable.Add(listener);
        }

        ~MenuViewModel()
        {
            ObjectCountManager.CountDown(GetType());
        }

        public void Initialize()
        {
        }

        #endregion

        #region Properties

        #region Title変更通知プロパティ

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value)
                    return;
                _Title = value;
                RaisePropertyChanged();
            }
        }

        #endregion

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

        #region Buttons変更通知プロパティ

        private ObservableSynchronizedCollection<MenuItemViewModel> _Buttons;

        public ObservableSynchronizedCollection<MenuItemViewModel> Buttons
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
                if (Math.Abs(_ButtonsContainerWidth - value) <
                    ConstValues.TOLERANCE)
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
                if (Math.Abs(_ButtonsContainerHeight - value) <
                    ConstValues.TOLERANCE)
                    return;
                _ButtonsContainerHeight = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsMenuVisible変更通知プロパティ

        private bool _IsMenuVisible;

        public bool IsMenuVisible
        {
            get { return _IsMenuVisible; }
            set
            {
                if (_IsMenuVisible == value)
                    return;
                _IsMenuVisible = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        public System.Windows.Point ButtonsOrigin { get; set; }

        #endregion
    }
}