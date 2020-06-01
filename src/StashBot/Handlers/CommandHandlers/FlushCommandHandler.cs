using System;
using StashBot.Data;
using StashBot.Models.ReturnModels.CommandHandlerReturnModels;

namespace StashBot.Handlers.CommandHandlers
{
    public class FlushCommandHandler
    {
        public static FlushCommandHandlerReturn Invoke(
            int userId
        )
        {
            FlushCommandHandlerReturn returnModel = new FlushCommandHandlerReturn { };

            if (AuthorData.CanAuthorFlushQueue(userId))
            {
                QueueData.DeleteRemovedQueueItems();
                returnModel.Status = FlushCommandHandlerReturn.FlushCommandReturnStatus.Success;
            }
            else
            {
                returnModel.Status = FlushCommandHandlerReturn.FlushCommandReturnStatus.NotAuthorized;
            }

            return returnModel;
        }
    }
}