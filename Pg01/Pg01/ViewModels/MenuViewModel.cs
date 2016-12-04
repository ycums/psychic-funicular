#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging.Windows;
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

        public void UpdateBasic(
            object sender,
            PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var model = sender as Model;

            if (model != null)
            {
                var pos = Cursor.Position;
                UpdateBasicCore(model, pos);
            }
        }

        public void UpdateBasicCore(Model model, Point pos)
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
                            x => new MenuItemViewModel(model, x, ButtonsOrigin))
                        .ToArray());

            X = pos.X - ButtonsContainerWidth/2;
            Y = pos.Y - ButtonsContainerHeight/2;
        }

        #endregion

        #region Initialize & Finalize

        public MenuViewModel(Model model)
        {
            ViewModelManager.AddEntryViewModel(this);
            ObjectCountManager.CountUp(GetType());

            _model = model;
            _X = -99999; //ちらつき防止
        }

        ~MenuViewModel()
        {
            ObjectCountManager.CountDown(GetType());
        }

        public void Initialize()
        {
            var listener = new PropertyChangedEventListener(_model)
            {
                {() => _model.Basic, UpdateBasic},
                {() => _model.IsMenuVisible, IsMenuVisibleChangedEventHandler}
            };
            CompositeDisposable.Add(listener);
            UpdateBasic(_model, null);
        }

        private void IsMenuVisibleChangedEventHandler(
            object sender,
            PropertyChangedEventArgs e)
        {
            var model = sender as Model;

            if (model != null)
            {
                if (model.IsMenuVisible)
                {
                    UpdateBasic(sender, e);
                }
                else
                {
                    DispatcherHelper.UIDispatcher.BeginInvoke(
                        (Action)
                        (() =>
                        {
                            Messenger.Raise(
                                new WindowActionMessage(
                                    WindowAction.Close,
                                    "WindowAction"));
                        }));
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("MenuViewModel Disposed");
            _model.IsMenuVisible = false;
            base.Dispose(disposing);
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

        public System.Windows.Point ButtonsOrigin { get; set; }

        #endregion

        #region Commands

        #region OnWindowClosedCommand

        private ViewModelCommand _OnWindowClosedCommand;

        public ViewModelCommand OnWindowClosedCommand
            => _OnWindowClosedCommand ??
               (_OnWindowClosedCommand =
                   new ViewModelCommand(OnWindowClosed));

        public void OnWindowClosed()
        {
            CompositeDisposable.Dispose();
            ViewModelManager.RemoveEntryViewModel(this);
        }

        #endregion

        #endregion
    }
}