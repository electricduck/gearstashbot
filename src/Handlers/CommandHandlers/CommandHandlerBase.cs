using System.Threading.Tasks;
using GearstashBot.Exceptions;
using GearstashBot.I18n;
using GearstashBot.Models.ArgumentModels;

namespace GearstashBot.Handlers.CommandHandlers
{
    public class CommandHandlerBase
    {
        public async static Task HandleNoPermission(
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