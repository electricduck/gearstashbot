using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using GearstashBot.Data;
using GearstashBot.Exceptions;
using GearstashBot.I18n;
using GearstashBot.Models;
using GearstashBot.Models.ArgumentModels;
using GearstashBot.Services;
using GearstashBot.Utilities;

namespace GearstashBot.Handlers.CommandHandlers
{
    public class ToolsCommandHandler : CommandHandlerBase
    {
        public static void Invoke(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                var toolsKeyboard = new InlineKeyboardMarkup(new[] {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData($"🔀 {Localization.GetPhrase(Localization.Phrase.RandomizeQueue, arguments.TelegramUser)}", $"tools_randomizequeue"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData($"🔫 {Localization.GetPhrase(Localization.Phrase.FlushDanglingUsers, arguments.TelegramUser)}", $"tools_purgeusers")
                    }
                });

                TelegramApiService.SendTextMessage(
                    new SendTextMessageArguments
                    {
                        ReplyMarkup = toolsKeyboard,
                        Text = $"⚒ <b>{Localization.GetPhrase(Localization.Phrase.Tools, arguments.TelegramUser)}</b>"
                    },
                    Program.BotClient,
                    arguments.TelegramMessageEvent
                );
            }
            else
            {
                throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.NoPermissionTools, arguments.TelegramUser));
                //await HandleNoPermission(arguments);
            }
        }

        public async static void InvokePurgeUsers(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                if (AuthorData.CanAuthorManageAuthors(arguments.TelegramUser.Id))
                {
                    List<Author> allAuthors = AuthorData.GetAuthors();
                    List<Author> authorsToDelete = new List<Author>();

                    foreach (var author in allAuthors)
                    {
                        int queueItemAmount = author.QueueItems.Count;

                        if (
                            author.CanDeleteOthers == false &&
                            author.CanManageAuthors == false &&
                            author.CanQueue == false &&
                            queueItemAmount == 0
                        )
                        {
                            authorsToDelete.Add(author);
                        }
                    }

                    if (authorsToDelete.Count == 0)
                    {
                        MessageUtilities.SendWarningAlert(Localization.GetPhrase(Localization.Phrase.NoDanglingUsers, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                    }
                    else
                    {
                        AuthorData.DeleteAuthorRange(authorsToDelete);
                        MessageUtilities.SendSuccessAlert(Localization.GetPhrase(
                            Localization.Phrase.FlushedXDanglingUsers,
                            arguments.TelegramUser,
                            new string[] {
                                        authorsToDelete.Count.ToString()
                            }
                        ),
                        arguments.TelegramCallbackQueryEvent);
                    }
                }
                else
                {
                    MessageUtilities.SendWarningAlert(Localization.GetPhrase(Localization.Phrase.NoPermissionFlushDanglingUsers, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                }
            }
            else
            {
                await HandleNoPermission(arguments);
            }
        }

        public async static Task InvokeRandomizeQueue(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                if (AuthorData.CanAuthorRandomizeQueue(arguments.TelegramUser.Id))
                {
                    Constants.IsPostingDisabled = true;
                    QueueData.RandomizeQueuedQueueItems();
                    Constants.IsPostingDisabled = false;
                    MessageUtilities.SendSuccessAlert(Localization.GetPhrase(Localization.Phrase.RandomizedQueue, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                }
                else
                {
                    MessageUtilities.SendWarningAlert(Localization.GetPhrase(Localization.Phrase.NoPermissionRandomizeQueue, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                }
            }
            else
            {
                await HandleNoPermission(arguments);
            }
        }
    }
}