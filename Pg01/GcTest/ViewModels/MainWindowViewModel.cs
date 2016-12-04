#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using GcTest.Models;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;

#endregion

namespace GcTest.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

        #region  Fields

        private readonly Model _model;

        #endregion

        #region Initialize & Finalize

        public MainWindowViewModel() : this(new Model())
        {
        }

        private MainWindowViewModel(Model model)
        {
            _model = model;
        }

        public void Initialize()
        {
            var listener = new PropertyChangedEventListener(_model)
            {
                {() => _model.MyProperty, MyPropertyChangedHandler}
            };
            CompositeDisposable.Add(listener);
        }

        private void MyPropertyChangedHandler(
            object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            MyProperty = _model.MyProperty;
        }

        #endregion

        #region Properties

        #region MyProperty変更通知プロパティ

        private string _MyProperty;


        public string MyProperty
        {
            get { return _MyProperty; }
            set
            {
                if (_MyProperty == value)
                    return;
                _MyProperty = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region DoCommand

        private ViewModelCommand _DoCommand;

        public ViewModelCommand DoCommand => _DoCommand ??
                                             (_DoCommand =
                                                 new ViewModelCommand(Do));

        public void Do()
        {
            var vm = new MenuViewModel(_model);
            Messenger.Raise(
                new TransitionMessage(vm, "OpenMenuMessage"));
        }

        #endregion

        #region GcCommand

        private ViewModelCommand _GcCommand;

        public ViewModelCommand GcCommand
            => _GcCommand ?? (_GcCommand = new ViewModelCommand(Gc));

        public void Gc()
        {
            Debug.WriteLine("GC");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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

        #region AllCloseCommand

        private ViewModelCommand _AllCloseCommand;

        public ViewModelCommand AllCloseCommand => _AllCloseCommand ??
                                                   (_AllCloseCommand =
                                                       new ViewModelCommand(
                                                           AllClose));

        public void AllClose()
        {
            _model.AllClose();
        }

        #endregion

        #endregion
    }
}