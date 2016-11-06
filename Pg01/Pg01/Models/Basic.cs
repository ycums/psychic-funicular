using System;
using Livet;

namespace Pg01.Models
{
    [Serializable]
    public class Basic : NotificationObject
    {
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
    }
}