#region

using GcTest.AppUtil;
using Livet;

#endregion

namespace GcTest.Models
{
    public class Model : NotificationObject
    {
        #region Public Functions

        public void CountUp()
        {
            ClickCount += 1;
        }

        #endregion

        #region  Initialize & Finalize

        public Model()
        {
            NotifyDebugInfo.WriteLine("Create Model");
        }

        ~Model()
        {
            NotifyDebugInfo.WriteLine("Destructor Model");
        }

        #endregion

        #region Properties

        #region ClickCount変更通知プロパティ

        private int _ClickCount;

        public int ClickCount
        {
            get { return _ClickCount; }
            set
            {
                if (_ClickCount == value)
                    return;
                _ClickCount = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion
    }
}