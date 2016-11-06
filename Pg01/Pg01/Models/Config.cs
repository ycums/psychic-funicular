using System;
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

        public Config()
        {
            Basic = new Basic {Title = "Title"};
        }

        public Basic Basic { get; set; }

        public bool LoadFile(string path)
        {
            try
            {
                var tmp = ConfigUtil.LoadConfigFile(path);
                Basic.Title = tmp.Basic.Title;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
    }

    [Serializable]
    public class Basic : NotificationObject
    {
        public string Title { get; set; }
    }
}