using System;
using StashBot.Data;
using StashBot.Models;
using StashBot.Models.ReturnModels.CommandHandlerReturnModels;
using StashBot.Models.ReturnModels.ServiceReturnModels;
using StashBot.Services;

namespace StashBot.Handlers.CommandHandlers
{
    public class PostCommandHandler
    {
        public static PostCommandHandlerReturn Invoke(
            string[] arguments,
            TelegramUser user
        )
        {
            PostCommandHandlerReturn returnModel = new PostCommandHandlerReturn { };
            QueueServiceReturn queueServiceReturn = null;

            if (AuthorData.CanAuthorQueue(user.Id))
            {
                if (
                    arguments[0].StartsWith("https://mobile.twitter.com") ||
                    arguments[0].StartsWith("https://twitter.com") ||
                    arguments[0].StartsWith("https://instagram.com") ||
                    arguments[0].StartsWith("https://www.instagram.com")
                ) // TODO: Don't check here; only in the QueueService
                {
                    queueServiceReturn = QueueService.QueueLink(
                        url: arguments[0],
                        mediaIndex: (arguments.Length == 2) ? (Convert.ToInt32(arguments[1]) - 1) : 0,
                        author: user
                    );

                    switch (queueServiceReturn.Status)
                    {
                        case QueueServiceReturn.QueueServiceReturnStatus.Duplicate:
                            returnModel.Status = PostCommandHandlerReturn.PostCommandReturnStatus.Duplicate;
                            break;
                        case QueueServiceReturn.QueueServiceReturnStatus.Queued:
                            returnModel.Status = PostCommandHandlerReturn.PostCommandReturnStatus.Success;
                            break;
                        case QueueServiceReturn.QueueServiceReturnStatus.SourceUrlNotFound:
                            returnModel.Status = PostCommandHandlerReturn.PostCommandReturnStatus.NotFound;
                            break;
                    }
                }
                else
                {
                    returnModel.Status = PostCommandHandlerReturn.PostCommandReturnStatus.ServiceNotSupported;
                }
            }
            else
            {
                returnModel.Status = PostCommandHandlerReturn.PostCommandReturnStatus.NotAuthorized;
            }

            return returnModel;
        }
    }
}