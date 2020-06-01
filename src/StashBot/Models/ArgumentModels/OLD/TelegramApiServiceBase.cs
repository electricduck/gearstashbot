using Telegram.Bot.Types.ReplyMarkups;

namespace StashBot.Models.ArgumentModels
{
    public class TelegramApiServiceBase
    {
        public long ChatId { get; set; } = 0;
        public IReplyMarkup ReplyMarkup { get; set; }
    }
}