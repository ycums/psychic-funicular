using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Livet;
using Pg01.Behaviors.Util;
using Pg01.Properties;

namespace Pg01.Models
{
    [Serializable]
    public class Model : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        #region Initialize & Finalize

        public Model()
        {
            var config = ConfigUtil.LoadDefaultConfigFile();
            Basic = config.Basic;
            ApplicationGroups = new List<ApplicationGroup>();
            _stateMachine = new StateMachine();
        }

        #endregion

        #region Fields

        private readonly StateMachine _stateMachine;
        private Bank _bank;

        #endregion

        #region Functions

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
            var entries = _bank.Entries;
            var result = _stateMachine.Exec(entries, e.KeyCode, e.UpDown);
            switch (result.Status)
            {
                case ExecStatus.LoadGroup:
                    LoadBank(result.NextBank);
                    break;
                case ExecStatus.Error:
                    MessageBox.Show(result.Message, Resources.MainWindow_ExecItemAction_Error,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            e.Cancel = result.ShouldCancel;
        }

        private void LoadApplicationGroup(string exeName, string windowText)
        {
            var q1 =
                ApplicationGroups.Where(ag => string.IsNullOrWhiteSpace(ag.MatchingRoule.ExeName) ||
                                              string.Equals(ag.MatchingRoule.ExeName, exeName,
                                                  StringComparison.CurrentCultureIgnoreCase))
                    .Where(ag => ag.MatchingRoule.WindowTitlePatterns.Exists(p => Util.Util.Like(windowText, p)));
            ApplicationGroup = q1.FirstOrDefault();
        }

        private void LoadBank(string bankName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        public Config Config { get; set; }

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

        #endregion
    }
}