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
                        InlineKeyboardButton.WithCallbackData("🔄 Refresh Profile", $"tools_refreshprofile")
                    }
                    /*,
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("🔀 Shuffle Queue", $"tools_shuffle")
                    },
                    */
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
                string statusText = "";

                if (AuthorData.CanAuthorFlushQueue(arguments.TelegramUser.Id))
                {
                    QueueData.DeleteRemovedQueueItems();
                    statusText = MessageUtilities.CreateSuccessMessage("Flushed removed posts");
                }
                else
                {
                    statusText = MessageUtilities.CreateWarningMessage($"You do not have permission to flush removed posts");
                }

                await Program.BotClient.AnswerCallbackQueryAsync(
                    callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id,
                    text: statusText
                );
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

        public async static Task InvokeRefreshProfile(CommandHandlerArguments arguments)
        {
            if (AuthorData.DoesAuthorExist(arguments.TelegramUser))
            {
                Author refreshedAuthor = AuthorData.UpdateAuthorTelegramProfile(arguments.TelegramUser);
                await Program.BotClient.AnswerCallbackQueryAsync(
                    callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id,
                    text: MessageUtilities.CreateSuccessMessage($"Refreshed profile. Hello {refreshedAuthor.TelegramName}!")
                );
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