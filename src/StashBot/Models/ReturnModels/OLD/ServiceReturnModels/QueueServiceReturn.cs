
namespace StashBot.Models.ReturnModels.ServiceReturnModels
{
    public class QueueServiceReturn
    {
        public QueueServiceReturnStatus Status { get; set; }

        public enum QueueServiceReturnStatus
        {
            Duplicate = 1,
            Queued = 0,
            SourceUrlNotFound = 2,
            ServiceNotSupported = 3,
        }
    }
}