#region

using System;
using GcTest.AppUtil;
using GcTest.Models;
using Livet;
using Livet.Commands;
using Livet.Messaging;

#endregion

namespace GcTest.ViewModels

{
    public class MainWindowViewModel : ViewModel
    {
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

        private readonly Model _model;

        #region Initalize & Finalize

        public MainWindowViewModel()
        {
            NotifyDebugInfo.WriteLine("Create ViewModel: MainWindowViewModel");

            _model = new Model();
        }


        ~MainWindowViewModel()
        {
            NotifyDebugInfo.WriteLine(
                "Destructor ViewModel: MainWindowViewModel");
        }

        #endregion

        #region Commands

        #region DoGcCommand

        private ViewModelCommand _DoGcCommand;

        public ViewModelCommand DoGcCommand
            => _DoGcCommand ?? (_DoGcCommand = new ViewModelCommand(DoGc));


        public void DoGc()
        {
            NotifyDebugInfo.WriteLine("=== GC 実行 ===");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion

        #region CreateDetailWindowCommand

        private ViewModelCommand _CreateDetailWindowCommand;

        public ViewModelCommand CreateDetailWindowCommand
            =>
            _CreateDetailWindowCommand ??
            (_CreateDetailWindowCommand =
                new ViewModelCommand(CreateDetailWindow));


        public void CreateDetailWindow()
        {
            // インスタンスのメッセンジャーを通じて MainWindow へメッセージを通知します。
            Messenger.Raise(
                new TransitionMessage(
                    new DetailWindowViewModel(_model, this), "Transition"));

            // 「すべて閉じる」ボタンの実行可否が変化したことを通知します
            CloseDetailWindowsCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region CloseDetailWindowsCommand

        private ViewModelCommand _CloseDetailWindowsCommand;

        public ViewModelCommand CloseDetailWindowsCommand
            =>
            _CloseDetailWindowsCommand ??
            (_CloseDetailWindowsCommand =
                new ViewModelCommand(CloseDetailWindows, CanCloseDetailWindows))
            ;

        public bool CanCloseDetailWindows()
        {
            return ViewModelManager.Count(typeof(DetailWindowViewModel)) != 0;
        }

        public void CloseDetailWindows()
        {
            ViewModelManager.CloseViewModels(typeof(DetailWindowViewModel));
        }

        #endregion

        #region CountUpCommand

        private ViewModelCommand _CountUpCommand;

        public ViewModelCommand CountUpCommand => _CountUpCommand ??
                                                  (_CountUpCommand =
                                                      new ViewModelCommand(
                                                          CountUp));

        public void CountUp()
        {
            _model.CountUp();
        }

        #endregion

        #endregion
    }
}