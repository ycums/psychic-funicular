using System;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Pg01.Views
{
    /* 
	 * ViewModelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedWeakEventListenerや
     * CollectionChangedWeakEventListenerを使うと便利です。独自イベントの場合はLivetWeakEventListenerが使用できます。
     * クローズ時などに、LivetCompositeDisposableに格納した各種イベントリスナをDisposeする事でイベントハンドラの開放が容易に行えます。
     *
     * WeakEventListenerなので明示的に開放せずともメモリリークは起こしませんが、できる限り明示的に開放するようにしましょう。
     */

    /// <summary>
    ///     MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {
        private NotifyIcon _notifyIcon;

        public MainWindow()
        {
            Application_Startup();

            InitializeComponent();

            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
#if !DEBUG
            Topmost = true;
#endif
            InitNotifyIcon();
            CaptureMouse();
        }

        private void Application_Startup()
        {
            var ipc = new IpcMessageSender<string>("RadGest", "RadGest01Startup");

            if (ipc.IsCadet)
            {
                Console.WriteLine(Properties.Resources.MainWindow_Application_Startup_);
                ipc.SendMessage($"{DateTime.Now:HH時mm分ss秒}です。後続プロセスが起動しました。");
                Environment.Exit(1);
            }
            else
            {
                // IPCでオブジェクトを受信したとき発生します。
                Console.WriteLine(Properties.Resources.MainWindow_Application_Startup_先行プロセス);
                ipc.MessageReceived += ipc_MessageReceived;
            }
            Console.ReadLine();
        }

        private void InitNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Text = Properties.Resources.NotifyIconText,
                Icon = Properties.Resources.PerfCenterCpl,
                Visible = true
            };

            var menuStrip = new ContextMenuStrip();

            var configItem = new ToolStripMenuItem {Text = Properties.Resources.NotifyIconconfigItemText};
            menuStrip.Items.Add(configItem);
            configItem.Click += configItem_Click;

            var exitItem = new ToolStripMenuItem {Text = Properties.Resources.NotifyIconExitItemText};
            menuStrip.Items.Add(exitItem);
            exitItem.Click += exitItem_Click;

            _notifyIcon.ContextMenuStrip = menuStrip;
            _notifyIcon.MouseClick += _notifyIcon_MouseClick;
        }

#region "IPC Events"

        private void ipc_MessageReceived(object sender, MessageEventArgs<string> e)
        {
            Dispatcher.Invoke(Show);
        }

#endregion

#region "NotifiIcon Events"

        private void _notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
            }
            catch
            {
                // ignored
            }
        }

        private void exitItem_Click(object sender, EventArgs e)
        {
            try
            {
                _notifyIcon.Dispose();
                Application.Current.Shutdown();
            }
            catch
            {
                // ignored
            }
        }

        private void configItem_Click(object sender, EventArgs e)
        {
        }

#endregion
    }
}