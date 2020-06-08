using System;
using System.Collections.Generic;
using StashBot.Data;
using StashBot.Exceptions;
using StashBot.I18n;
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
                  Explanation = "Link to post that includes photo/video",
                  Name = "Link",
                  Position = 1
              },
              new HelpArgument {
                  Example = "1, 3, 7",
                  Explanation = "If link includes multiple photos/videos, select which item to post (defaults to <i>1</i>)",
                  Name = "Media Index",
                  Optional = true,
                  Position = 2
              }
            },
            Command = "post",
            Description = @"Post links to queue, automatically grabbing media and relevant data

Supported services:
• Flickr (https://www.flickr.com)
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
                        MessageUtilities.SendSuccessMessage(Localization.GetPhrase(Localization.Phrase.PostSuccessfullyQueued, arguments.TelegramUser), arguments.TelegramMessageEvent);
                        break;
                    case QueueServiceReturn.QueueServiceReturnStatus.Duplicate:
                        throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.AlreadyBeenQueued, arguments.TelegramUser));
                    case QueueServiceReturn.QueueServiceReturnStatus.SourceUrlNotFound:
                        throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.LinkContainsNoMedia, arguments.TelegramUser));
                    case QueueServiceReturn.QueueServiceReturnStatus.ServiceNotSupported:
                        throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.ServiceNotSupported, arguments.TelegramUser));
                }
            }
            else
            {
                throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.NoPermissionPostQueue, arguments.TelegramUser));
            }
        }
    }
}