using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Livet;
using Pg01.Behaviors.Util;
using Pg01.Properties;

namespace Pg01.Models
{
    [Serializable]
    public class Config : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        #region Initialize & Finalize

        public Config()
        {
            Basic = new Basic
            {
                Title = "Title01",
                WindowLocation = new Location {X = 99999, Y = 0},
                Buttons = new List<ButtonItem>
                {
                    new ButtonItem {X = 0, Y = 0, Key = "Num9"},
                    new ButtonItem {X = 0, Y = 1, Key = "Num6"},
                    new ButtonItem {X = 0, Y = 2, Key = "Num3"},
                    new ButtonItem {X = 0, Y = 3, Key = "Num5"},
                    new ButtonItem {X = 8, Y = 8, Key = "Num8"}
                }
            };
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
                var tmp = ConfigUtil.LoadConfigFile(path);
                Basic = tmp.Basic;
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
                ConfigUtil.SaveConfigFile(this, path);
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

        private void LoadBank(string resultNextGroup)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

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