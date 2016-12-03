#region

using System.Windows;
using Livet;

#endregion

namespace GcTest.Models
{
    public class Model : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        #region Public Functions

        public void CountUp()
        {
            Count += 1;
        }

        #endregion

        public void AllClose()
        {
            ChilidVisibility = Visibility.Hidden;
        }

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

        #region ChilidVisibility変更通知プロパティ

        private Visibility _ChilidVisibility;

        public Visibility ChilidVisibility
        {
            get { return _ChilidVisibility; }
            set
            {
                if ((_ChilidVisibility == value) &&
                    (value != Visibility.Hidden))
                    return;
                _ChilidVisibility = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Count変更通知プロパティ

        private int _Count;

        public int Count
        {
            get { return _Count; }
            set
            {
                if (_Count == value)
                    return;
                _Count = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion
    }
}