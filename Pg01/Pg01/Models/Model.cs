﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Livet;
using Pg01.Models.Util;
using Pg01.Views.Behaviors.Util;

namespace Pg01.Models
{
    [Serializable]
    public class Model : NotificationObject
    {
        #region Initialize & Finalize

        public Model()
        {
            _skc = new SendKeyCode();
            _stateMachine = new StateMachine();
            var config = ConfigUtil.LoadDefaultConfigFile();
            Basic = config.Basic;
            ApplicationGroups = config.ApplicationGroups;
        }

        #endregion

        #region Fields

        private readonly StateMachine _stateMachine;
        [UsedImplicitly] private int _keySending;
        private readonly SendKeyCode _skc;

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
                Debug.WriteLine(ex.Message);
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
            Debug.WriteLine($"{e.KeyCode} {e.UpDown}");
            var result = _stateMachine.Exec(_Bank.Entries, e.KeyCode, e.UpDown);
            e.Cancel = result.ShouldCancel;
            ProcessExecResult(result);
        }

        public void ProcAction(ActionItem action, NativeMethods.KeyboardUpDown kud)
        {
            var result = _stateMachine.ExecCore(action, kud);
            ProcessExecResult(result);
        }

        public void LoadApplicationGroup(string exeName, string windowText)
        {
            var q1 =
                ApplicationGroups.Where(ag => string.IsNullOrWhiteSpace(ag.MatchingRoule.ExeName) ||
                                              string.Equals(ag.MatchingRoule.ExeName, exeName,
                                                  StringComparison.CurrentCultureIgnoreCase))
                    .Where(ag => ag.MatchingRoule.WindowTitlePatterns.Exists(p => Util.Util.Like(windowText, p)));
            ApplicationGroup = q1.FirstOrDefault();
        }

        #endregion

        #region Private Functions

        private void ProcessExecResult(ExecResult result)
        {
            switch (result.ActionType)
            {
                case ActionType.Key:
                    if (_keySending != 0)
                    {
                        _keySending++;
                        _skc.SendKey(result.ActionValue, result.UpDown);
                        _keySending--;
                    }
                    break;

                case ActionType.Send:
                    if ((_keySending != 0) && (0 < result.ActionValue.Length))
                        try
                        {
                            _keySending++;
                            _skc.SendWait(result.ActionValue);
                        }
                        catch (ArgumentException aex)
                        {
                            Message = aex.Message;
                        }
                        finally
                        {
                            _keySending--;
                        }
                    break;
            }
            switch (result.Status)
            {
                case ExecStatus.LoadGroup:
                    LoadBank(_ApplicationGroup, result.NextBank);
                    break;
            }
        }

        private void LoadBank(ApplicationGroup applicationGroup, string bankName)
        {
            if (bankName == null)
                bankName = "";
            Bank = applicationGroup.Banks.Find(x => x.Name == bankName);
        }

        #endregion

        #region Properties

        private Config Config { get; set; }

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

        public List<ApplicationGroup> ApplicationGroups { get; set; }

        #endregion

        #region Message変更通知プロパティ

        private string _Message;

        public string Message
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

        #endregion
    }
}