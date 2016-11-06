using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace Pg01.Models
{
    public class Model : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */
        public Basic Basic { get; set; }

        public bool LoadFile(string mResponse)
        {
            return true;
        }
    }

    public class Basic: NotificationObject
    {
        public string Title { get; set; }
    }
}
