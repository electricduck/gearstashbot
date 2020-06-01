using System;
using System.Linq;
using Telegram.Bot.Args;
using StashBot.Data;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Models.ReturnModels.CommandHandlerReturnModels;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class UserCommandHandler
    {
        public static UserCommandHandlerReturn Invoke(
            string action,
            string userId,
            string option,
            string value,
            MessageEventArgs telegramMessageEvent
        )
        {
            UserCommandHandlerReturn returnModel = new UserCommandHandlerReturn { };

            long userIdInt = 0;
            bool valueBool = value.ToLower() == "true" ? true : false;

            try
            {
                userIdInt = Convert.ToInt64(userId);
            }
            catch (Exception) { }

            Author author = AuthorData.GetAuthor(userIdInt, false);

            if (AuthorData.CanAuthorManageAuthors(TelegramUtilities.GetUserId(telegramMessageEvent)))
            {
                switch (action)
                {
                    case "create":
                        if (author == null)
                        {
                            AuthorData.CreateAuthor(userIdInt);
                            returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.CreatedUser;
                        }
                        else
                        {
                            returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.UserAlreadyExists;
                        }
                        break;
                    case "delete":
                        if (author != null)
                        {
                            if (TelegramUtilities.GetUserId(telegramMessageEvent) != author.TelegramId)
                            {
                                AuthorData.DeleteAuthor(author.TelegramId);
                                returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.DeletedUser;
                            }
                            else
                            {
                                returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.CannotDeleteSelf;
                            }
                        }
                        else
                        {
                            returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.UserNotFound;
                        }
                        break;
                    case "get":
                        if (author != null)
                        {
                            string canDeleteOthers = author.CanDeleteOthers ? "✔️" : "✖️";
                            string canFlushQueue = author.CanFlushQueue ? "✔️" : "✖️";
                            string canManageAuthors = author.CanManageAuthors ? "✔️" : "✖️";
                            string canQueue = author.CanQueue ? "✔️" : "✖️";

                            returnModel.SendTextMessageArguments = new SendTextMessageArguments
                            {
                                Text = $@"<b>{author.TelegramId}</b>
—
<b>⌨️ Permissions</b>
• {canDeleteOthers} CanDeleteOthers
• {canFlushQueue} CanFlushQueue
• {canManageAuthors} CanManageAuthors
• {canQueue} CanQueue"
                            };

                            returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.GotUser;
                        }
                        else
                        {
                            returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.UserNotFound;
                        }
                        break;
                    case "set":
                        if (author != null)
                        {
                            switch (option.ToLower())
                            {
                                case "candeleteothers":
                                    AuthorData.SetAuthorDeleteOthersPermission(author.TelegramId, valueBool);
                                    returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.EditedUserPermissions;
                                    break;
                                case "canflush":
                                    AuthorData.SetAuthorFlushQueuePermission(author.TelegramId, valueBool);
                                    returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.EditedUserPermissions;
                                    break;
                                case "canmanageauthors":
                                    if (TelegramUtilities.GetUserId(telegramMessageEvent) != author.TelegramId)
                                    {
                                        AuthorData.SetAuthorManageAuthorsPermission(author.TelegramId, valueBool);
                                        returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.EditedUserPermissions;
                                    }
                                    else
                                    {
                                        returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.CannotChangeForSelf;
                                    }
                                    break;
                                case "canqueue":
                                    AuthorData.SetAuthorQueuePermission(author.TelegramId, valueBool);
                                    returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.EditedUserPermissions;
                                    break;
                                default:
                                    MessageUtilities.PrintInfoMessage(option.ToLower());
                                    returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.InvalidArgs;
                                    break;
                            }
                        }
                        else
                        {
                            returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.UserNotFound;
                        }
                        break;
                    default:
                        returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.InvalidArgs;
                        break;
                }
            }
            else
            {
                returnModel.Status = UserCommandHandlerReturn.UserCommandReturnStatus.Unauthorized;
            }

            return returnModel;
        }
    }
}