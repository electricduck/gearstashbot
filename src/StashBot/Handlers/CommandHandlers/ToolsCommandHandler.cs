using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using StashBot.Exceptions;
using StashBot.Data;
using StashBot.I18n;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class ToolsCommandHandler
    {
        public static Help Help = new Help
        {
            Command = "tools",
            Description = "Extra tools to manage the bot"
        };

        public static void Invoke(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                var toolsKeyboard = new InlineKeyboardMarkup(new[] {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData($"🔄 {Localization.GetPhrase(Localization.Phrase.RefreshProfile, arguments.TelegramUser)}", $"tools_refreshprofile")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData($"🔀 {Localization.GetPhrase(Localization.Phrase.RandomizeQueue, arguments.TelegramUser)}", $"tools_randomizequeue"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData($"🚽 {Localization.GetPhrase(Localization.Phrase.FlushRemovedPosts, arguments.TelegramUser)}", $"tools_flush"),
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

        public async static Task InvokeFlush(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                if (AuthorData.CanAuthorFlushQueue(arguments.TelegramUser.Id))
                {
                    QueueData.DeleteRemovedQueueItems();
                    MessageUtilities.AlertSuccessMessage(Localization.GetPhrase(Localization.Phrase.FlushedRemovedPosts, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                }
                else
                {
                    MessageUtilities.AlertWarningMessage(Localization.GetPhrase(Localization.Phrase.NoPermissionFlushRemovedPosts, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                }
            }
            else
            {
                await HandleNoPermission(arguments);
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
                            author.CanFlushQueue == false &&
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
                        MessageUtilities.AlertWarningMessage(Localization.GetPhrase(Localization.Phrase.NoDanglingUsers, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                    }
                    else
                    {
                        AuthorData.DeleteAuthorRange(authorsToDelete);
                        MessageUtilities.AlertSuccessMessage(Localization.GetPhrase(
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
                    MessageUtilities.AlertWarningMessage(Localization.GetPhrase(Localization.Phrase.NoPermissionFlushDanglingUsers, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                }
            }
            else
            {
                await HandleNoPermission(arguments);
            }
        }

        public async static Task InvokeRefreshProfile(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                Author refreshedAuthor = AuthorData.UpdateAuthorTelegramProfile(arguments.TelegramUser);
                MessageUtilities.AlertSuccessMessage(Localization.GetPhrase(
                    Localization.Phrase.RefreshedProfileHelloX,
                    arguments.TelegramUser,
                    new string[] {
                        refreshedAuthor.TelegramName
                    }
                ),
                arguments.TelegramCallbackQueryEvent);
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
                    MessageUtilities.AlertSuccessMessage(Localization.GetPhrase(Localization.Phrase.RandomizedQueue, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                }
                else
                {
                    MessageUtilities.AlertWarningMessage(Localization.GetPhrase(Localization.Phrase.NoPermissionRandomizeQueue, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                }
            }
            else
            {
                await HandleNoPermission(arguments);
            }
        }

        private async static Task HandleNoPermission(
            CommandHandlerArguments arguments,
            Localization.Phrase phrase = Localization.Phrase.NoPermissionTools
        )
        {
            if (arguments.TelegramCallbackQueryEvent != null)
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                );
            }

            throw new CommandHandlerException(Localization.GetPhrase(phrase, arguments.TelegramUser));
        }
    }
}