#region

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using JetBrains.Annotations;
using Livet;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using Pg01.Models;

#endregion

namespace Pg01.ViewModels
{
    public class MenuViewModel : ViewModel
    {
        #region Private Functions

        public void UpdateBasic(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var model = sender as Model;

            if (model != null)
            {
                var items = model.Menu.MenuItem;
                ButtonsOrigin = new Point(-items.Max(x => Math.Abs(x.X)), -items.Max(x => Math.Abs(x.Y)));
                ButtonsContainerWidth = (-ButtonsOrigin.X*2 + 1)*ConstValues.ButtonWidth;
                ButtonsContainerHeight = (-ButtonsOrigin.Y*2 + 1)*ConstValues.ButtonHeight;
                Buttons =
                    new ObservableSynchronizedCollection<MenuItemViewModel>(
                        model.Menu.MenuItem.Select(x => new MenuItemViewModel(model, x, ButtonsOrigin)).ToArray());

                var pos = Cursor.Position;
                X = pos.X - Width/2;
                Y = pos.Y - Height/2;
            }
        }

        #endregion

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
            _X = -99999; //ちらつき防止
        }

        public void Initialize()
        {
            _listener = new PropertyChangedEventListener(_model)
            {
                {() => _model.Basic, UpdateBasic},
                {() => _model.IsMenuVisible, Closing}
            };
            UpdateBasic(_model, null);
        }

        private void Closing(object sender, PropertyChangedEventArgs e)
        {
            var model = sender as Model;

            if ((model != null) && !model.IsMenuVisible)
            {
                DispatcherHelper.UIDispatcher.BeginInvoke((Action)(() =>
                {
                    Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));
                }));
            }
        }

        protected override void Dispose(bool disposing)
        {
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

        public Point ButtonsOrigin { get; set; }

        #endregion

        #region Commands

        #endregion
    }
}