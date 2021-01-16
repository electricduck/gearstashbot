
using System;
using GearstashBot.Models;
using Telegram.Bot.Args;

namespace GearstashBot.Utilities
{
    public class TelegramUtilities
    {
        public static TelegramUser GetTelegramUser(MessageEventArgs telegramMessageEvent)
        {
            return new TelegramUser
            {
                Id = telegramMessageEvent.Message.From.Id,
                Language = telegramMessageEvent.Message.From.LanguageCode,
                Name = telegramMessageEvent.Message.From.FirstName + " " + telegramMessageEvent.Message.From.LastName,
                Username = "@" + telegramMessageEvent.Message.From.Username
            };
        }

        public static TelegramUser GetTelegramUser(CallbackQueryEventArgs telegramCallbackQueryEvent)
        {
            return new TelegramUser
            {
                Id = telegramCallbackQueryEvent.CallbackQuery.From.Id,
                Language = telegramCallbackQueryEvent.CallbackQuery.From.LanguageCode,
                Name = telegramCallbackQueryEvent.CallbackQuery.From.FirstName + " " + telegramCallbackQueryEvent.CallbackQuery.From.LastName,
                Username = "@" + telegramCallbackQueryEvent.CallbackQuery.From.Username
            };
        }

        [Obsolete("Use GetTelegramUser()")]
        public static string GetUserLanguageCode(MessageEventArgs telegramMessageEvent)
        {
            return telegramMessageEvent.Message.From.LanguageCode;
        }

        [Obsolete("Use GetTelegramUser()")]
        public static string GetUser(MessageEventArgs telegramMessageEvent)
        {
            return ("@" + telegramMessageEvent.Message.From.Username);
        }

        [Obsolete("Use GetTelegramUser()")]
        public static int GetUserId(MessageEventArgs telegramMessageEvent)
        {
            return telegramMessageEvent.Message.From.Id;
        }

        [Obsolete("Use GetTelegramUser()")]
        public static string GetUserName(MessageEventArgs telegramMessageEvent)
        {
            return (telegramMessageEvent.Message.From.FirstName + " " + telegramMessageEvent.Message.From.LastName);
        }
    }
}