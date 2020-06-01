using System;
using StashBot.Data;
using StashBot.Models.ReturnModels.CommandHandlerReturnModels;
using StashBot.Models.ReturnModels.ServiceReturnModels;
using StashBot.Services;

namespace StashBot.Handlers.CommandHandlers
{
    public class PostCommandHandler
    {
        public static PostCommandHandlerReturn Invoke(
            string[] arguments,
            int authorId,
            string authorName,
            string authorUsername
        )
        {
            PostCommandHandlerReturn returnModel = new PostCommandHandlerReturn { };
            QueueServiceReturn queueServiceReturn = null;

            if (AuthorData.CanAuthorQueue(authorId))
            {
                if (
                    arguments[0].StartsWith("https://mobile.twitter") ||
                    arguments[0].StartsWith("https://twitter")
                )
                {
                    switch (arguments.Length)
                    {
                        // TODO: Parse this better
                        case 1:
                            queueServiceReturn = QueueService.QueueLinkPost(
                                url: arguments[0],
                                authorId: authorId,
                                authorName: authorName,
                                authorUsername: authorUsername
                            );
                            break;
                        case 2:
                            queueServiceReturn = QueueService.QueueLinkPost(
                                url: arguments[0],
                                authorId: authorId,
                                authorName: authorName,
                                authorUsername: authorName,
                                mediaIndex: (Convert.ToInt32(arguments[1]) - 1)
                            );
                            break;
                            /*case 3:
                                QueueService.PostLink(arguments[0],
                                    mediaIndex: (Convert.ToInt32(arguments[1]) + 1),
                                    name: arguments[2]
                                );
                                break;*/
                    }

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