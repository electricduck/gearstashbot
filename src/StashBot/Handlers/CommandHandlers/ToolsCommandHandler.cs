using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using StashBot.Exceptions;
using StashBot.Data;
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
            Description = "Extra tools to manage StashBot"
        };

        public static void Invoke(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                var toolsKeyboard = new InlineKeyboardMarkup(new[] {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("🚽 Flush Removed Posts", $"tools_flush"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("🔫 Purge Dangling Users", $"tools_purgeusers")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("🔄 Refresh Profile", $"tools_refreshprofile")
                    }
                });

                TelegramApiService.SendTextMessage(
                    new SendTextMessageArguments
                    {
                        ReplyMarkup = toolsKeyboard,
                        Text = "⚒ <b>Tools</b>"
                    },
                    Program.BotClient,
                    arguments.TelegramMessageEvent
                );
            }
            else
            {
                throw new CommandHandlerException("You do not have permission to use tools");
            }
        }

        public async static Task InvokeFlush(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                if (AuthorData.CanAuthorFlushQueue(arguments.TelegramUser.Id))
                {
                    QueueData.DeleteRemovedQueueItems();
                    MessageUtilities.AlertSuccessMessage("Flushed removed posts", arguments.TelegramCallbackQueryEvent);
                }
                else
                {
                    MessageUtilities.AlertWarningMessage($"You do not have permission to flush removed posts", arguments.TelegramCallbackQueryEvent);
                }
            }
            else
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                );

                throw new CommandHandlerException("You do not have permission to use tools");
            }
        }

        public static void InvokePurgeUsers(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                if (AuthorData.CanAuthorManageAuthors(arguments.TelegramUser.Id))
                {
                    Task.Run(() =>
                    {
                        try
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
                                MessageUtilities.AlertWarningMessage("No dangling users to purge", arguments.TelegramCallbackQueryEvent);
                            }
                            else
                            {
                                AuthorData.DeleteAuthorRange(authorsToDelete);
                                MessageUtilities.AlertSuccessMessage($"Purged {authorsToDelete.Count} dangling users", arguments.TelegramCallbackQueryEvent);
                            }
                        }
                        catch (Exception e)
                        {
                            MessageUtilities.SendErrorMessage(e, arguments.TelegramCallbackQueryEvent);
                        }
                    });
                }
                else
                {
                    MessageUtilities.AlertWarningMessage("You do not have permission to purge dangling users", arguments.TelegramCallbackQueryEvent);
                    //throw new CommandHandlerAlertException("You do not have permission to purge dangling users");
                }
            }
            else
            {
                throw new CommandHandlerException("You do not have permission to use tools");
            }
        }

        public async static Task InvokeRefreshProfile(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                Author refreshedAuthor = AuthorData.UpdateAuthorTelegramProfile(arguments.TelegramUser);
                MessageUtilities.AlertSuccessMessage($"Refreshed profile. Hello {refreshedAuthor.TelegramName}!", arguments.TelegramCallbackQueryEvent);
            }
            else
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                );

                throw new CommandHandlerException("You do not have permission to use tools");
            }
        }
    }
}