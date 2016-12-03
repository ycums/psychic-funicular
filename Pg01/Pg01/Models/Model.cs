#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using JetBrains.Annotations;
using Livet;
using Pg01.Models.Util;
using Pg01.Views.Behaviors.Util;

#endregion

namespace Pg01.Models
{
    public class Model : NotificationObject
    {
        #region Initialize & Finalize

        public Model()
            : this(ConfigUtil.LoadDefaultConfigFile(), new SendKeyCode())
        {
        }

        public Model(Config config, ISendKeyCode skc)
        {
            _skc = skc;
            _stateMachine = new StateMachine();
            _WindowInfo = new WindowInfo("", "");
            Config = config;
            _timer = new Timer(100);
            _timer.Elapsed += _timer_Elapsed;

            OnMouse = false;
            AutoHide = true;
        }

        #endregion

        #region Fields

        private readonly Timer _timer;
        private readonly StateMachine _stateMachine;
        [UsedImplicitly] private int _keySending;
        private readonly ISendKeyCode _skc;

        #endregion

        #region Public Functions

        public bool LoadFile(string path)
        {
            try
            {
                Config = ConfigUtil.LoadConfigFile(path);
            }
            catch (Exception ex)
            {
                Message =
                    new Message(
                        "File Loading Error", MessageBoxImage.Error, ex.Message);
                return false;
            }
            return true;
        }

        public bool SaveFile(string path)
        {
            try
            {
                ConfigUtil.SaveConfigFile(Config, path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public void SetEvent(KeyboardHookedEventArgs e)
        {
            if (_keySending != 0) return;

            if (_ApplicationGroup.Name != "")
            {
                Debug.WriteLine($"{e.KeyCode} {e.UpDown}");
            }
            var result =
                _stateMachine.Exec(
                    _Bank.Entries, e.KeyCode, e.UpDown,
                    IsMenuVisible);
            e.Cancel = result.ShouldCancel;
            ProcessExecResult(result);
        }

        public void ProcAction(ActionItem action,
            NativeMethods.KeyboardUpDown kud)
        {
            var result = _stateMachine.ExecCore(action, kud);
            ProcessExecResult(result);
        }

        #endregion

        #region Private Functions

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var exeName = Path.GetFileName(ForegroundWidowInfo.GetExeName());
            var windowText = ForegroundWidowInfo.GetWindowText();
            WindowInfo = new WindowInfo(exeName, windowText);
        }

        private void ProcessExecResult(ExecResult result)
        {
            switch (result.ActionType)
            {
                case ActionType.Key:
                    if (_keySending == 0)
                    {
                        _keySending++;
                        _skc.SendKey(result.ActionValue, result.UpDown);
                        _keySending--;
                    }
                    break;

                case ActionType.Send:
                    if ((_keySending == 0) && (0 < result.ActionValue.Length))
                        try
                        {
                            _keySending++;
                            _skc.SendWait(result.ActionValue);
                        }
                        catch (Exception ex)
                        {
                            Message = new Message("Command Error",
                                MessageBoxImage.Error, ex.Message);
                        }
                        finally
                        {
                            _keySending--;
                        }
                    break;
                case ActionType.Menu:
                    LoadMenu(_ApplicationGroup, result.ActionValue);
                    IsMenuVisible = true;
                    break;
                case ActionType.System:
                    switch (result.ActionValue)
                    {
                        case ConstValues.SystemCommandCancel:
                            if (IsMenuVisible)
                                IsMenuVisible = false;
                            else
                            {
                                LoadBank(_ApplicationGroup, "");
                            }
                            break;
                        case ConstValues.SystemCommandReloadConfig:
                            LoadFile(ConfigUtil.GetConfigFilePath());
                            break;
                        case ConstValues.SystemCommandToggleAutoHide:
                            AutoHide = !AutoHide;
                            break;
                    }
                    break;
            }
            switch (result.Status)
            {
                case ExecStatus.LoadBank:
                    if (result.ActionType != ActionType.Menu)
                    {
                        LoadBank(_ApplicationGroup, result.NextBank);
                        IsMenuVisible = false;
                    }
                    break;
            }
        }

        private void LoadConfig(Config config)
        {
            Basic = config.Basic;
            ApplicationGroups = config.ApplicationGroups;
        }

        private void LoadMenu(
            ApplicationGroup applicationGroup,
            string menuName)
        {
            if (menuName == null)
                menuName = "";
            Menu = applicationGroup.Menus.Find(x => x.Name == menuName);
        }

        private void LoadBank(
            ApplicationGroup applicationGroup,
            string bankName)
        {
            if (bankName == null) return;
            Bank = applicationGroup.Banks.Any()
                ? applicationGroup.Banks.Find(x => x.Name == bankName)
                : new Bank
                {
                    Entries = new List<Entry>(),
                    Name = ""
                };
        }

        private void LoadApplicationGroup(WindowInfo windowInfo)
        {
            var q1 =
                ApplicationGroups.Where(
                        ag =>
                            string.IsNullOrWhiteSpace(ag.MatchingRoule.ExeName) ||
                            string.Equals(ag.MatchingRoule.ExeName,
                                windowInfo.ExeName,
                                StringComparison.CurrentCultureIgnoreCase))
                    .Where(
                        ag =>
                            ag.MatchingRoule.WindowTitlePatterns.Exists(
                                p => Util.Util.Like(windowInfo.WindowText, p)));
            var groups = q1 as ApplicationGroup[] ?? q1.ToArray();
            ApplicationGroup = groups.Any()
                ? groups.FirstOrDefault()
                : new ApplicationGroup
                {
                    Name = "",
                    MatchingRoule = new MatchingRoule(),
                    Banks = new List<Bank>(),
                    Menus = new List<Menu>()
                };
        }

        private void UpdateMainWindowVisibility()
        {
            if (string.IsNullOrEmpty(_ApplicationGroup.Name))
                MainWindowVisibility = Visibility.Hidden;
            else if (_OnMouse && _autoHide)
                MainWindowVisibility = Visibility.Hidden;
            else MainWindowVisibility = Visibility.Visible;
        }

        #endregion

        #region Properties

        #region Config変更通知プロパティ

        private Config _Config;

        public Config Config
        {
            get { return _Config; }
            set
            {
                if (_Config == value)
                    return;
                _Config = value;

                LoadConfig(_Config);

                RaisePropertyChanged();
            }
        }

        #endregion

        #region ApplicationGroup変更通知プロパティ

        private ApplicationGroup _ApplicationGroup;

        public ApplicationGroup ApplicationGroup
        {
            get { return _ApplicationGroup; }
            set
            {
                if (_ApplicationGroup == value)
                    return;
                _ApplicationGroup = value;
                LoadBank(_ApplicationGroup, "");
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Bank変更通知プロパティ

        private Bank _Bank;

        public Bank Bank
        {
            get { return _Bank; }
            set
            {
                if (_Bank == value)
                    return;
                _Bank = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Basic変更通知プロパティ

        private Basic _Basic;

        public Basic Basic
        {
            get { return _Basic; }
            set
            {
                if (_Basic == value)
                    return;
                _Basic = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ApplicationGroups変更通知プロパティ

        private List<ApplicationGroup> _ApplicationGroups;

        public List<ApplicationGroup> ApplicationGroups
        {
            get { return _ApplicationGroups; }
            set
            {
                if (_ApplicationGroups == value)
                    return;
                _ApplicationGroups = value;
                LoadApplicationGroup(_WindowInfo);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Message変更通知プロパティ

        private Message _Message;

        public Message Message
        {
            get { return _Message; }
            set
            {
                if (_Message == value)
                    return;
                _Message = value;
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
                _IsMenuVisible = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Menu変更通知プロパティ

        private Menu _Menu;

        public Menu Menu
        {
            get { return _Menu; }
            set
            {
                if (_Menu == value)
                    return;
                _Menu = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region WindowInfo変更通知プロパティ

        private WindowInfo _WindowInfo;
        private Visibility _mainWindowVisibility;
        private bool _autoHide;

        public WindowInfo WindowInfo
        {
            get { return _WindowInfo; }
            set
            {
                if ((_WindowInfo.ExeName == value.ExeName) &&
                    (_WindowInfo.WindowText == value.WindowText)) return;
                _WindowInfo = value;
                Debug.WriteLine(
                    $"{_WindowInfo.ExeName}: {_WindowInfo.WindowText}");
                LoadApplicationGroup(_WindowInfo);
                UpdateMainWindowVisibility();
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Timer変更通知プロパティ

        public bool TimerEnabled
        {
            get { return _timer.Enabled; }
            set
            {
                if (_timer.Enabled == value)
                    return;
                _timer.Enabled = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        public Visibility MainWindowVisibility
        {
            get { return _mainWindowVisibility; }
            set
            {
                if (_mainWindowVisibility == value) return;
                _mainWindowVisibility = value;
                RaisePropertyChanged();
            }
        }

        public bool AutoHide
        {
            get { return _autoHide; }
            set
            {
                if (_autoHide == value) return;
                _autoHide = value;
                UpdateMainWindowVisibility();
                RaisePropertyChanged();
            }
        }

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
                UpdateMainWindowVisibility();
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion
    }
}