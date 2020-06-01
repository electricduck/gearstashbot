
namespace StashBot.Models.ReturnModels.CommandHandlerReturnModels
{
    public class PostCommandHandlerReturn : CommandHandlerReturnBase
    {
        public PostCommandReturnStatus Status { get; set; }

        public enum PostCommandReturnStatus
        {
            Duplicate = 2,
            NotAuthorized = 1,
            NotFound = 3,
            ServiceNotSupported = 4,
            Success = 0
        }
    }
}