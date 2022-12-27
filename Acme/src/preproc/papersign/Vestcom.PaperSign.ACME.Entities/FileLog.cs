namespace Vestcom.PaperSign.ACME.Entities
{
    public class FileLog
    {
        public string OrderNumber { get; set; }
        public FileStatus Status { get; set; }
        public string ErrorMesssage { get; set; }
        public int FileLogKey { get; set; }
    }

    public enum FileStatus
    {
        ProcessPreProcInProgress=1,
        ProcessPreProcCompleted=2,
        ProcessPreProcFailed=3
    }
}
