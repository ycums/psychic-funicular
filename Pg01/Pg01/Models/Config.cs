using System;
using System.Collections.Generic;
using System.Diagnostics;
using Livet;

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
            Basic = new Basic()
            {
                Title = "Title01",
                Buttons = new List<ButtonItem>()
                {
                    new ButtonItem() { X=0,Y=0, Key = "Num9"},
                    new ButtonItem() { X=0,Y=1, Key = "Num6"},
                    new ButtonItem() { X=0,Y=2, Key = "Num3"},
                    new ButtonItem() { X=0,Y=3, Key = "Num5"},

                }
            };

            //<buttons> <button x="0" y="0" Key ="Num9" />
            //  <button x="0" y="1" Key ="Num6" />
            //  <button x="0" y="2" Key ="Num3" />
            //  <button x="0" y="3" Key ="Num5" />
            //</buttons>
        }

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

        #endregion

        #endregion
    }
}