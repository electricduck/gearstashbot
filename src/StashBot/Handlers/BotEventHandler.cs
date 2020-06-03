using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using StashBot.Data;
using StashBot.Exceptions;
using StashBot.Handlers.CommandHandlers;
using StashBot.Models;
using StashBot.Models.ReturnModels.CommandHandlerReturnModels;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers
{
    public class BotEventHandler
    {
        public static void Bot_OnMessage(object sender, MessageEventArgs telegramMessageEvent)
        {
            Task.Run(() =>
            {
                string command = null;
                string[] arguments = null;

                try
                {
                    if (telegramMessageEvent.Message.Text != null)
                    {
                        string messageText = telegramMessageEvent.Message.Text.ToString();

                        Regex commandAndArgumentsRegex = new Regex(@"^([\/][a-z]{1,10})([ ])*([\/a-zA-Z0-9_:., ]*)$");
                        Match parsedCommand = commandAndArgumentsRegex.Match(messageText);

                        TelegramUser user = new TelegramUser {
                            Id = TelegramUtilities.GetUserId(telegramMessageEvent),
                            Language = TelegramUtilities.GetUserLanguageCode(telegramMessageEvent),
                            Name = TelegramUtilities.GetUserName(telegramMessageEvent),
                            Username = TelegramUtilities.GetUser(telegramMessageEvent)
                        };

                        if (messageText.StartsWith("/"))
                        {
                            command = parsedCommand.Groups[1].Value.Replace("/", "");
                            arguments = parsedCommand.Groups[3].Value.Split(" ");

                            if(String.IsNullOrEmpty(arguments[0].ToString()))
                            {
                                arguments = null;
                            }

                            switch (command)
                            {
                                case "catpls":
                                    var catPlsCommandResult = CatPlsCommandHandler.Invoke();
                                    if (catPlsCommandResult != null)
                                    {
                                        TelegramApiService.SendPhoto(
                                            catPlsCommandResult.SendPhotoArguments,
                                            Program.BotClient,
                                            telegramMessageEvent
                                        );
                                    }
                                    break;

                                case "info":
                                    var infoCommandResult = InfoCommandHandler.Invoke(telegramMessageEvent);
                                    if (infoCommandResult != null)
                                    {
                                        TelegramApiService.SendTextMessage(
                                            infoCommandResult.SendTextMessageArguments,
                                            Program.BotClient,
                                            telegramMessageEvent
                                        );
                                    }
                                    break;

                                case "view":
                                    var viewCommandResult = ViewCommandHandler.Invoke(
                                        arguments: arguments,
                                        telegramMessageEvent: telegramMessageEvent
                                    );
                                    if (viewCommandResult != null)
                                    {
                                        if (viewCommandResult.SendPhotoArguments != null)
                                        {
                                            TelegramApiService.SendPhoto(
                                                viewCommandResult.SendPhotoArguments,
                                                Program.BotClient,
                                                telegramMessageEvent
                                            );
                                        }
                                        else if(viewCommandResult.SendVideoArguments != null)
                                        {
                                            TelegramApiService.SendVideo(
                                                viewCommandResult.SendVideoArguments,
                                                Program.BotClient,
                                                telegramMessageEvent
                                            );
                                        }
                                    }
                                    break;

                                case "start":
                                    int authorCount = AuthorData.CountAuthors();
                                    if (authorCount == 0)
                                    {
                                        AuthorData.CreateAuthor(user.Id);

                                        AuthorData.SetAuthorDeleteOthersPermission(user.Id, true);
                                        AuthorData.SetAuthorFlushQueuePermission(user.Id, true);
                                        AuthorData.SetAuthorManageAuthorsPermission(user.Id, true);
                                        AuthorData.SetAuthorQueuePermission(user.Id, true);

                                        MessageUtilities.PrintInfoMessage($"Created first author ({user.Id}) with all permissions");
                                        MessageUtilities.SendSuccessMessage($"Created first author with all permissions,", telegramMessageEvent);
                                    }
                                    break;

                                case "err":
                                    throw new Exception("Manually triggered exception");

                                // TODO: Change below cases to match with above cases
                                case "flush":
                                    FlushCommandHandlerReturn flushCommandHandlerReturn = FlushCommandHandler.Invoke(user.Id);
                                    switch (flushCommandHandlerReturn.Status)
                                    {
                                        case FlushCommandHandlerReturn.FlushCommandReturnStatus.NotAuthorized:
                                            MessageUtilities.SendWarningMessage("You do not have permission to flush removed posts", telegramMessageEvent);
                                            break;
                                        case FlushCommandHandlerReturn.FlushCommandReturnStatus.Success:
                                            MessageUtilities.SendSuccessMessage("Flushed removed posts", telegramMessageEvent);
                                            break;
                                    }
                                    break;
                                case "post":
                                    PostCommandHandlerReturn postCommandHandlerReturn = PostCommandHandler.Invoke(arguments, user);
                                    switch (postCommandHandlerReturn.Status)
                                    {
                                        case PostCommandHandlerReturn.PostCommandReturnStatus.Duplicate:
                                            MessageUtilities.SendWarningMessage("This has already been queued", telegramMessageEvent);
                                            break;
                                        case PostCommandHandlerReturn.PostCommandReturnStatus.NotAuthorized:
                                            MessageUtilities.SendWarningMessage("You do not have permission to queue new posts", telegramMessageEvent);
                                            break;
                                        case PostCommandHandlerReturn.PostCommandReturnStatus.NotFound:
                                            MessageUtilities.SendWarningMessage("This link contains no media or does not exist", telegramMessageEvent);
                                            break;
                                        case PostCommandHandlerReturn.PostCommandReturnStatus.ServiceNotSupported:
                                            MessageUtilities.SendWarningMessage("This service is not supported", telegramMessageEvent);
                                            break;
                                        case PostCommandHandlerReturn.PostCommandReturnStatus.Success:
                                            MessageUtilities.SendSuccessMessage("Post successfully queued", telegramMessageEvent);
                                            break;
                                    }
                                    break;
                                    /*case "user":
                                        UserCommandHandlerReturn userCommandHandlerReturn = UserCommandHandler.Invoke(
                                            arguments[0],
                                            (arguments.Length >= 2 ? arguments[1] : String.Empty),
                                            (arguments.Length >= 3 ? arguments[2] : String.Empty),
                                            (arguments.Length == 4 ? arguments[3] : String.Empty),
                                            telegramMessageEvent
                                        );
                                        switch (userCommandHandlerReturn.Status)
                                        {
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.GotUser:
                                                TelegramApiService.SendTextMessage(
                                                    userCommandHandlerReturn.SendTextMessageArguments,
                                                    Program.BotClient,
                                                    telegramMessageEvent
                                                );
                                                break;
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.CannotChangeForSelf:
                                                MessageUtilities.SendWarningMessage("You cannot change this permission for yourself", telegramMessageEvent);
                                                break;
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.CannotDeleteSelf:
                                                MessageUtilities.SendWarningMessage("You cannot delete yourself", telegramMessageEvent);
                                                break;
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.CreatedUser:
                                                MessageUtilities.SendSuccessMessage("Successfully created user", telegramMessageEvent);
                                                break;
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.DeletedUser:
                                                MessageUtilities.SendSuccessMessage("Successfully deleted user", telegramMessageEvent);
                                                break;
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.EditedUserPermissions:
                                                MessageUtilities.SendSuccessMessage("Successfully edited user permission", telegramMessageEvent);
                                                break;
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.InvalidArgs:
                                                MessageUtilities.SendWarningMessage("Invalid arguments: see /help", telegramMessageEvent); // TODO: Help
                                                break;
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.Unauthorized:
                                                MessageUtilities.SendWarningMessage("You do not have permission to manage users", telegramMessageEvent);
                                                break;
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.UserAlreadyExists:
                                                MessageUtilities.SendWarningMessage("This ID already exists", telegramMessageEvent);
                                                break;
                                            case UserCommandHandlerReturn.UserCommandReturnStatus.UserNotFound:
                                                MessageUtilities.SendWarningMessage("This ID could not be found", telegramMessageEvent);
                                                break;
                                        }
                                        break;*/
                            }
                        }

                    }
                }
                catch(ArgumentException)
                {
                    MessageUtilities.SendWarningMessage($"Invalid arguments: see <code>/help {command}</code> for details", telegramMessageEvent);
                }
                catch(CommandHandlerException e)
                {
                    MessageUtilities.SendWarningMessage(e.Message, telegramMessageEvent);
                }
                catch (Exception e)
                {
                    MessageUtilities.SendErrorMessage(e, telegramMessageEvent);
                }
            });
        }

        public async static void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            try
            {
                string[] commands = callbackQueryEventArgs.CallbackQuery.Data.Split(":");

                switch (commands[0])
                {
                    case "view_next":
                    case "view_prev":
                        await ViewCommandHandler.InvokeChange(
                            callbackQueryEventArgs, Convert.ToInt32(commands[1])
                        );
                        break;
                    case "view_del":
                        await ViewCommandHandler.InvokeDelete(
                            callbackQueryEventArgs, Convert.ToInt32(commands[1])
                        );
                        break;
                }
            }
            catch (Exception e)
            {
                MessageUtilities.SendErrorMessage(e, callbackQueryEventArgs);
            }
        }

        public static void Bot_OnInlineQueryRecieved(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine("Hello!");
        }
    }
}