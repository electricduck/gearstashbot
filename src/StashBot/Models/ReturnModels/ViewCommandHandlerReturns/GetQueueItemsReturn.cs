
using Telegram.Bot.Types.ReplyMarkups;

namespace StashBot.Models.ReturnModels.ViewCommandHandlerReturns
{
    public class GetQueueItemsReturn
    {
        public QueueItem SelectedQueuedItem { get; set; }
        public QueueItem PreviousQueuedItem { get; set; }
        public QueueItem NextQueuedItem { get; set; }
        public InlineKeyboardMarkup Keyboard { get; set; }
        public string Caption { get; set; }
        public bool HasItems { get; set; }
    }
}