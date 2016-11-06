using System.Windows;
using JetBrains.Annotations;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Pg01.Models;

namespace Pg01.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Initialize & Finalize

        public void Initialize()
        {
            _listener = new PropertyChangedEventListener(_config)
            {
                () => _config.Basic.Title,
                (_, __) => RaisePropertyChanged(() => Title)
            };
        }

        #endregion

        #region Fields

        private readonly Config _config = new Config();
        [UsedImplicitly] private PropertyChangedEventListener _listener;

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

        #endregion

        #region Commands

        #region OpenCommand

        private ListenerCommand<OpeningFileSelectionMessage> _OpenCommand;

        public ListenerCommand<OpeningFileSelectionMessage> OpenCommand
            => _OpenCommand ?? (_OpenCommand = new ListenerCommand<OpeningFileSelectionMessage>(Open, CanOpen));

        public bool CanOpen()
        {
            return true;
        }

        public void Open(OpeningFileSelectionMessage m)
        {
            if (m.Response == null)
            {
                return;
            }
            if (!_config.LoadFile(m.Response[0]))
                Messenger.Raise(new InformationMessage("無効なファイル", "Error", MessageBoxImage.Error, "Info"));
        }

        #endregion

        #endregion
    }
}