using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using StashBot.Data;
using StashBot.Models.ArgumentModels;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class ToolsCommandHandler
    {
        public static void Invoke(CommandHandlerArguments arguments)
        {
            var toolsKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("🚽 Flush Removed Posts", $"tools_flush"),
                }/*,
                new []
                {
                    InlineKeyboardButton.WithCallbackData("🔀 Shuffle Queue", $"tools_shuffle")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("🔄 Update Author Details", $"tools_updateauthor")
                }*/
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

        public async static Task InvokeFlush(CommandHandlerArguments arguments)
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
    }
}