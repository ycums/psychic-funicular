#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using Pg01.Models;
using Pg01.Views.Behaviors.Util;
using Pg01Util;

#endregion

namespace Pg01.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Fields

        private readonly Model _model;
        private MenuViewModel _menuViewModel;

        #endregion

        #region Initialize & Finalize

        public MainWindowViewModel(Model model)
        {
            ObjectCountManager.CountUp(GetType());

            _model = model;
            _Buttons =
                new ObservableSynchronizedCollection<ButtonItemViewModel>();

            var listener = new PropertyChangedEventListener(_model)
            {
                {() => _model.Basic, UpdateBasic},
                {
                    () => _model.ApplicationGroup,
                    (s, e) =>
                            ApplicationGroupName = _model.ApplicationGroup.Name
                },
                {
                    () => _model.Bank,
                    (sender, args) =>
                        BankName =
                            string.IsNullOrEmpty(_model.Bank.Name)
                                ? "(default)"
                                : _model.Bank.Name
                },
                {() => _model.Message, MassageChanged},
                {
                    () => _model.MainWindowVisibility,
                    (s, e) =>
                            MainWindowVisibility = _model.MainWindowVisibility
                }
            };
            CompositeDisposable.Add(listener);
        }

        public MainWindowViewModel() : this(new Model())
        {
        }

        ~MainWindowViewModel()
        {
            ObjectCountManager.CountDown(GetType());
        }

        public void Initialize()
        {
            UpdateBasic(_model, null);

            _model.TimerEnabled = true;

            Observable.Range(0, 1)
                .Delay(TimeSpan.FromSeconds(0.5))
                .Subscribe(Hoge);
        }

        private void Hoge(int value)
        {
            if (_menuViewModel == null)
                _menuViewModel = new MenuViewModel(_model);
            DispatcherHelper.UIDispatcher.BeginInvoke(
                (Action)
                (() =>
                {
                    Messenger.Raise(
                        new TransitionMessage(
                            _menuViewModel, "OpenMenuMessage"));
                }));
        }

        #region Event Handelers

        private void MassageChanged(object sender, PropertyChangedEventArgs e)
        {
            var m = _model.Message;
            if (m != null)
            {
                Messenger.Raise(
                    new InformationMessage(
                        m.Text, m.Caption, m.Image, "Information"));
            }
        }

        #endregion

        #endregion

        #region Private Functions

        private void UpdateBasic(
            object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var model = sender as Model;

            if (model != null)
            {
                Title = model.Basic.Title;

                UpdateButtons(model, model.Basic.Buttons);

                ButtonsContainerHeight =
                    Buttons.Max(x => x.Y) + ConstValues.ButtonHeight;
                ButtonsContainerWidth =
                    Buttons.Max(x => x.X) + ConstValues.ButtonWidth;
                ButtonsAlignment = model.Basic.ButtonsAlignment;
                X = Math.Min(
                    Math.Max(0, model.Basic.WindowLocation.X),
                    SystemParameters.VirtualScreenWidth - Width);
                Y = Math.Min(
                    Math.Max(0, model.Basic.WindowLocation.Y),
                    SystemParameters.VirtualScreenHeight - Height);
            }
        }

        private void UpdateButtons(
            Model model, IEnumerable<ButtonItem> newButtons)
        {
            foreach (var button in Buttons)
            {
                button.AltDispose();
            }
            Buttons.Clear();
            foreach (var i in newButtons)
            {
                Buttons.Add(new ButtonItemViewModel(model, i));
            }
        }

        private void CorrectY()
        {
            Y = Math.Min(
                Math.Max(0, _model.Basic.WindowLocation.Y),
                SystemParameters.VirtualScreenHeight - Height);
        }

        private void CorrectX()
        {
            X = Math.Min(
                Math.Max(0, _model.Basic.WindowLocation.X),
                SystemParameters.VirtualScreenWidth - Width);
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

        #region ApplicationGroupName変更通知プロパティ

        private string _ApplicationGroupName;

        public string ApplicationGroupName
        {
            get { return _ApplicationGroupName; }
            set
            {
                if (_ApplicationGroupName == value)
                    return;
                _ApplicationGroupName = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region BankName変更通知プロパティ

        private string _BankName;

        public string BankName
        {
            get { return _BankName; }
            set
            {
                if (_BankName == value)
                    return;
                _BankName = value;
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

        #region ButtonsAlignMent変更通知プロパティ

        private HorizontalAlignment _buttonsAlignment;

        public HorizontalAlignment ButtonsAlignment
        {
            get { return _buttonsAlignment; }
            set
            {
                if (_buttonsAlignment == value)
                    return;
                _buttonsAlignment = value;
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

        #region Event変更通知プロパティ

        private KeyboardHookedEventArgs _Event;

        public KeyboardHookedEventArgs Event
        {
            get { return _Event; }
            set
            {
                if (_Event == value)
                    return;
                _Event = value;
                _model.SetEvent(_Event);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Visibility変更通知プロパティ

        private Visibility _mainWindowVisibility;

        public Visibility MainWindowVisibility
        {
            get { return _mainWindowVisibility; }
            set
            {
                if (_mainWindowVisibility == value)
                    return;
                _mainWindowVisibility = value;
                Debug.WriteLine(value);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region OnMouse変更通知プロパティ

        private bool _OnMouse;

        public bool OnMouse
        {
            get { return _OnMouse; }
            set
            {
                if (_OnMouse == value)
                    return;
                _OnMouse = value;
                Debug.WriteLine(value);
                _model.OnMouse = _OnMouse;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region OpenCommand

        private ListenerCommand<OpeningFileSelectionMessage> _OpenCommand;

        public ListenerCommand<OpeningFileSelectionMessage> OpenCommand
            =>
            _OpenCommand ??
            (_OpenCommand =
                new ListenerCommand<OpeningFileSelectionMessage>(Open, CanOpen))
            ;

        public bool CanOpen()
        {
            return true;
        }

        public void Open(OpeningFileSelectionMessage m)
        {
            if (m.Response == null)
                return;
            if (!_model.LoadFile(m.Response[0]))
                Messenger.Raise(
                    new InformationMessage(
                        "無効なファイル", "Error",
                        MessageBoxImage.Error, "Information"));
        }

        #endregion

        #region ReloadCommand

        private ViewModelCommand _ReloadCommand;

        public ViewModelCommand ReloadCommand
            =>
            _ReloadCommand ??
            (_ReloadCommand = new ViewModelCommand(Reload, CanReload));

        public bool CanReload()
        {
            return true;
        }

        public void Reload()
        {
            var path = ConfigUtil.GetConfigFilePath();
            _model.LoadFile(path);
        }

        #endregion

        #region SaveCommand

        private ListenerCommand<SavingFileSelectionMessage> _SaveCommand;

        public ListenerCommand<SavingFileSelectionMessage> SaveCommand
            =>
            _SaveCommand ??
            (_SaveCommand =
                new ListenerCommand<SavingFileSelectionMessage>(Save, CanSave))
            ;

        public bool CanSave()
        {
            return true;
        }

        public void Save(SavingFileSelectionMessage parameter)
        {
            if (parameter.Response == null)
                return;
            if (!_model.SaveFile(parameter.Response[0]))
                Messenger.Raise(
                    new InformationMessage(
                        "無効なファイル", "Error",
                        MessageBoxImage.Error, "Info"));
        }

        #endregion

        #region CloseCommand

        private ViewModelCommand _CloseCommand;

        public ViewModelCommand CloseCommand
            => _CloseCommand ?? (_CloseCommand = new ViewModelCommand(Close));


        public void Close()
        {
            Messenger.Raise(
                new WindowActionMessage(
                    WindowAction.Close,
                    "WindowAction"));
        }

        #endregion

        #region GcCommand

        private ViewModelCommand _GcCommand;

        public ViewModelCommand GcCommand
            => _GcCommand ?? (_GcCommand = new ViewModelCommand(Gc));

        public void Gc()
        {
            Debug.WriteLine("====== GC ======");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            ObjectCountManager.Dump();
        }

        #endregion

        #endregion
    }
}