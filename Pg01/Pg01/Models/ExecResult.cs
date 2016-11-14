using Pg01.Views.Behaviors.Util;

namespace Pg01.Models
{
    public class ExecResult
    {
        public ExecResult(bool shouldCancel) : this(shouldCancel, ExecStatus.None, string.Empty)
        {
        }

        public ExecResult(bool shoudCancel, ExecStatus status, string nextBank)
            : this(shoudCancel, status, nextBank, ActionType.None, "", NativeMethods.KeyboardUpDown.None)
        {
        }

        public ExecResult(bool shouldCancel,
            ExecStatus status, string nextBank, 
            ActionType actionType, string actionValue,
            NativeMethods.KeyboardUpDown kud)
        {
            ShouldCancel = shouldCancel;
            Status = status;
            NextBank = nextBank;
            ActionType = actionType;
            ActionValue = actionValue;
            UpDown = kud;
        }

        public NativeMethods.KeyboardUpDown UpDown { get; set; }

        public ActionType ActionType { get; set; }

        public bool ShouldCancel { get; set; }

        public ExecStatus Status { get; set; }

        public string NextBank { get; set; }
        public string ActionValue { get; set; }
    }
}