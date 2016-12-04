#region

using System.Collections.Generic;
using System.Windows;
using GcTest.AppUtil;
using GcTest.Models;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.Windows;

#endregion

namespace GcTest.ViewModels

{
    public class DetailWindowViewModel : ViewModel, IRaiseCloseMessage
    {
        #region IRaiseCloseMessage インターフェイス

        public void RaiseCloseMessage()
        {
            Messenger.Raise(
                new WindowActionMessage(WindowAction.Close, "Close"));
        }

        #endregion //IRaiseCloseMessage インターフェイス

        /*
         * ViewModelからViewを操作したい場合は、
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信してください。
         */

        /*
         * UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         */

        /*
         * Modelからの変更通知などの各種イベントをそのままViewModelで購読する事はメモリリークの
         * 原因となりやすく推奨できません。ViewModelHelperの各静的メソッドの利用を検討してください。
         */

        #region Fields

        private bool _canDoSomething = true;
        private readonly Model _model;

        #endregion

        #region Initialize & Finalize

        public DetailWindowViewModel(Model model, MainWindowViewModel parent)
        {
            _model = model;
            Parent = parent;
            ViewModelManager.AddEntryViewModel(this);
            NotifyDebugInfo.WriteLine(
                "Create ViewModel: DetailWindowViewModel");
            _TestMessage = "テスト";

            var listener = new PropertyChangedEventListener(model)
                {
                    {
                        () => model.ClickCount,
                        (sender, e) => ClickCountChangedEventHandler()
                    }
                };
            CompositeDisposable.Add(listener);
        }

        private void ClickCountChangedEventHandler()
        {
            if (_model != null) ClickCount = _model.ClickCount;
        }

        ~DetailWindowViewModel()
        {
            NotifyDebugInfo.WriteLine(
                "Destructor ViewModel: DetailWindowViewModel");
        }

        #endregion

        #region Properties

        #region ClickCount変更通知プロパティ

        private int _ClickCount;

        public int ClickCount
        {
            get { return _ClickCount; }
            set
            {
                if (_ClickCount == value)
                    return;
                _ClickCount = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        public MainWindowViewModel Parent { get; }

        #region OnClosedWindowCommand

        private ViewModelCommand _OnClosedWindowCommand;


        // Window で Closed イベントが起きた時に実行されるコマンド

        public ViewModelCommand OnClosedWindowCommand
            =>
            _OnClosedWindowCommand ??
            (_OnClosedWindowCommand = new ViewModelCommand(OnClosedWindow));


        public void OnClosedWindow()
        {
            // ViewModelManager への登録を解除します。
            ViewModelManager.RemoveEntryViewModel(this);
            if (ViewModelManager.Count(typeof(DetailWindowViewModel)) == 0)
            {
                // MainWindowViewModel の「すべて閉じる」ボタンの実行可否が変化したことを通知します
                Parent.CloseDetailWindowsCommand.RaiseCanExecuteChanged();
            }
            CompositeDisposable.Dispose();
        }

        #endregion

        #region DoSomethingCommand

        private ViewModelCommand _DoSomethingCommand;


        public ViewModelCommand DoSomethingCommand
            =>
            _DoSomethingCommand ??
            (_DoSomethingCommand =
                new ViewModelCommand(DoSomething, CanDoSomething));

        public bool CanDoSomething()
        {
            return _canDoSomething;
        }

        public void DoSomething()
        {
            // インスタンスのメッセンジャーを通じて DetailWindow へメッセージを通知します。

            Messenger.Raise(
                new InformationMessage(
                    "ボタンがクリックされました。", "確認", MessageBoxImage.Information, "Info"));

            _canDoSomething = false;

            // 「ボタン」ボタンの実行可否が変化したことを通知します
            DoSomethingCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region TestMessage変更通知プロパティ

        private string _TestMessage;

        public string TestMessage
        {
            get { return _TestMessage; }

            set
            {
                if (EqualityComparer<string>.Default.Equals(
                    _TestMessage, value))
                    return;
                _TestMessage = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion
    }
}