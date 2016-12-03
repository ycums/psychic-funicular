#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using GcTest.Models;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging.Windows;

#endregion

namespace GcTest.ViewModels
{
    public class MenuViewModel : ViewModel
    {
        #region Fields

        private readonly Model _model;

        #endregion

        #region EventHandlers

        private void ChildVisibilityChangedHandler(
            object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Debug.WriteLine(
                $"ChildVisibilityChangedHandler: {_model.ChilidVisibility}");
            DispatcherHelper.UIDispatcher.BeginInvoke((Action) Close);
        }

        private void CountChangedHandler(
            object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Debug.WriteLine($"{_model.Count}");
            Count = _model.Count;
        }

        #endregion

        #region Initialize & Finalize

        public MenuViewModel() : this(new Model())
        {
        }

        public MenuViewModel(Model model)
        {
            _model = model;
            _Visibility = Visibility.Visible;
        }

        public void Initialize()
        {
            var listener = new PropertyChangedEventListener(_model)
            {
                {() => _model.ChilidVisibility, ChildVisibilityChangedHandler},
                {() => _model.Count, CountChangedHandler}
            };
            CompositeDisposable.Add(listener);
        }


        ~MenuViewModel()
        {
            Debug.WriteLine("MenuViewModel Destructor");
        }

        #endregion

        #region Properties

        #region Visibility変更通知プロパティ

        private Visibility _Visibility;

        public Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                if (_Visibility == value)
                    return;
                _Visibility = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Count変更通知プロパティ

        private int _Count;

        public int Count
        {
            get { return _Count; }
            set
            {
                if (_Count == value)
                    return;
                _Count = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region CloseCommand

        private ViewModelCommand _CloseCommand;

        public ViewModelCommand CloseCommand => _CloseCommand ??
                                                (_CloseCommand =
                                                    new ViewModelCommand(Close))
            ;

        public void Close()
        {
            Messenger.Raise(
                new WindowActionMessage(WindowAction.Close, "WindowAction"));
        }

        #endregion

        #endregion
    }
}