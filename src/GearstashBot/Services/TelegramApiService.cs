using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using GearstashBot.Models.ArgumentModels;
using GearstashBot.Utilities;

namespace GearstashBot.Services
{
    public class TelegramApiService
    {
        public static async void SendPhoto(SendPhotoArguments args, ITelegramBotClient botClient, MessageEventArgs telegramMessageEvent)
        {

            try
            {
                await botClient.SendPhotoAsync(
                    caption: args.Caption,
                    chatId: ((args.ChatId != 0) ? args.ChatId : Convert.ToInt32(telegramMessageEvent.Message.Chat.Id)),
                    parseMode: ParseMode.Html,
                    photo: args.Photo,
                    replyMarkup: args.ReplyMarkup
                );
            }
            catch (Exception e)
            {
                MessageUtilities.PrintErrorMessage(e, Guid.Empty);
            }
        }

        public static async void SendTextMessage(SendTextMessageArguments args, ITelegramBotClient botClient, MessageEventArgs telegramMessageEvent)
        {
            try
            {
                var test = await botClient.SendTextMessageAsync(
                    chatId: ((args.ChatId != 0) ? args.ChatId : Convert.ToInt32(telegramMessageEvent.Message.Chat.Id)),
                    disableWebPagePreview: args.DisableWebPagePreview,
                    parseMode: ParseMode.Html,
                    replyMarkup: args.ReplyMarkup,
                    text: args.Text
                );
            }
            catch (Exception e)
            {
                MessageUtilities.PrintErrorMessage(e, Guid.Empty);
            }
        }

        public static async void SendVideo(SendVideoArguments args, ITelegramBotClient botClient, MessageEventArgs telegramMessageEvent)
        {

            try
            {
                await botClient.SendVideoAsync(
                    caption: args.Caption,
                    chatId: ((args.ChatId != 0) ? args.ChatId : Convert.ToInt32(telegramMessageEvent.Message.Chat.Id)),
                    parseMode: ParseMode.Html,
                    replyMarkup: args.ReplyMarkup,
                    video: args.Video
                );
            }
            catch (Exception e)
            {
                MessageUtilities.PrintErrorMessage(e, Guid.Empty);
            }
        }
    }
}