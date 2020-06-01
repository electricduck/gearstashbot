
namespace StashBot.Models.ReturnModels.CommandHandlerReturnModels
{
    public class FlushCommandHandlerReturn : CommandHandlerReturnBase
    {
        public FlushCommandReturnStatus Status { get; set; }

        public enum FlushCommandReturnStatus
        {
            NotAuthorized = 1,
            Success = 0
        }
    }
}