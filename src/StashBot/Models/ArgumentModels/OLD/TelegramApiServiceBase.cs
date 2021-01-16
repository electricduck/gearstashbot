using Telegram.Bot.Types.ReplyMarkups;

namespace GearstashBot.Models.ArgumentModels
{
    public class TelegramApiServiceBase
    {
        public long ChatId { get; set; } = 0;
        public IReplyMarkup ReplyMarkup { get; set; }
    }
}