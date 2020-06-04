using System;
using System.Collections.Generic;
using StashBot.Data;
using StashBot.Exceptions;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Models.ReturnModels.ServiceReturnModels;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class PostCommandHandler
    {
        public static Help Help = new Help {
            Arguments = new List<HelpArgument> {
              new HelpArgument {
                  Explanation = "Post including photo/video",
                  Name = "Link",
                  Position = 1
              },
              new HelpArgument {
                  Example = "1, 3, 7",
                  Explanation = "Selection of media from a gallery (if link includes one)",
                  Name = "Media Index",
                  Optional = true,
                  Position = 2
              }
            },
            Command = "post",
            Description = @"Post links to queue, automatically grabbing media and relevant data

Supported services:
• Instagram (https://www.instagram.com)
• Twitter (https://twitter.com)"
        };

        public static void Invoke(CommandHandlerArguments arguments)
        {
            if(arguments.CommandArguments == null)
            {
                throw new ArgumentException();
            }

            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                QueueServiceReturn queueLinkStatus = QueueService.QueueLink(
                    url: arguments.CommandArguments[0],
                    mediaIndex: (arguments.CommandArguments.Length == 2) ? (Convert.ToInt32(arguments.CommandArguments[1]) - 1) : 0,
                    user: arguments.TelegramUser
                );

                AuthorData.UpdateAuthorTelegramProfile(arguments.TelegramUser);

                switch (queueLinkStatus.Status)
                {
                    case QueueServiceReturn.QueueServiceReturnStatus.Queued:
                        MessageUtilities.SendSuccessMessage("Post successfully queued", arguments.TelegramMessageEvent);
                        break;
                    case QueueServiceReturn.QueueServiceReturnStatus.Duplicate:
                        throw new CommandHandlerException("This has already been queued");
                    case QueueServiceReturn.QueueServiceReturnStatus.SourceUrlNotFound:
                        throw new CommandHandlerException("This link contains no media or does not exist");
                    case QueueServiceReturn.QueueServiceReturnStatus.ServiceNotSupported:
                        throw new CommandHandlerException("This service is not supported");
                }
            }
            else
            {
                throw new CommandHandlerException("You do not have permission to queue new posts");
            }
        }
    }
}