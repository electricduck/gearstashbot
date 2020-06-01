
namespace StashBot.Models.ReturnModels.CommandHandlerReturnModels
{
    public class ViewCommandHandlerReturn : CommandHandlerReturnBase
    {
        public ViewCommandReturnStatus Status { get; set; }

        public enum ViewCommandReturnStatus
        {
            Success
        }
    }
}