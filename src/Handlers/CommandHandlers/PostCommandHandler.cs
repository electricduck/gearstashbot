using System;
using System.Collections.Generic;
using GearstashBot.Data;
using GearstashBot.Exceptions;
using GearstashBot.I18n;
using GearstashBot.Models;
using GearstashBot.Models.ArgumentModels;
using GearstashBot.Models.ReturnModels.ServiceReturnModels;
using GearstashBot.Services;
using GearstashBot.Utilities;

namespace GearstashBot.Handlers.CommandHandlers
{
    public class PostCommandHandler
    {
        public static void Invoke(CommandHandlerArguments arguments)
        {
            if (arguments.CommandArguments == null)
            {
                throw new ArgumentException();
            }

            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                if (!Constants.IsPostingDisabled)
                {
                    QueueServiceReturn queueLinkStatus = QueueService.QueueLink(
                        url: arguments.CommandArguments[0],
                        mediaIndex: (arguments.CommandArguments.Length == 2) ? (Convert.ToInt32(arguments.CommandArguments[1]) - 1) : 0,
                        user: arguments.TelegramUser
                    );

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
                    throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.CannotPostTemporarilyDueToLongRunningRequest, arguments.TelegramUser));
                }
            }
            else
            {
                throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.NoPermissionPostQueue, arguments.TelegramUser));
            }
        }
    }
}