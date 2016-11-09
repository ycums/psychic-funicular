namespace Pg01.Models
{
    public class ExecResult
    {
        public ExecResult(bool shouldCancel) : this(shouldCancel, ExecStatus.None, string.Empty)
        {
        }

        public ExecResult(bool shoudCancel, ExecStatus status, string nextGroup, string message = "")
        {
            ShouldCancel = shoudCancel;
            Status = status;
            NextGroup = nextGroup;
            Message = message;
        }

        public bool ShouldCancel { get; set; }

        public string Message { get; set; }

        public ExecStatus Status { get; set; }

        public string NextGroup { get; set; }
    }
}