
using Telegram.Bot.Args;

namespace StashBot.Utilities
{
    public class TelegramUtilities
    {
        public static string GetUserLanguageCode(MessageEventArgs telegramMessageEvent)
        {
            return telegramMessageEvent.Message.From.LanguageCode;
        }

        public static string GetUser(MessageEventArgs telegramMessageEvent)
        {
            return ("@" + telegramMessageEvent.Message.From.Username);
        }

        public static int GetUserId(MessageEventArgs telegramMessageEvent)
        {
            return telegramMessageEvent.Message.From.Id;
        }

        public static string GetUserName(MessageEventArgs telegramMessageEvent)
        {
            return (telegramMessageEvent.Message.From.FirstName + " " + telegramMessageEvent.Message.From.LastName);
        }
    }
}