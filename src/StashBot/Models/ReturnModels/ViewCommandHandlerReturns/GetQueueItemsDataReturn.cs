
using Telegram.Bot.Types.ReplyMarkups;

namespace StashBot.Models.ReturnModels.ViewCommandHandlerReturns
{
    public class GetQueueItemsDataReturn
    {
        public QueueItem SelectedQueuedItem { get; set; }
        public QueueItem PreviousQueuedItem { get; set; }
        public QueueItem NextQueuedItem { get; set; }
        public bool IsEarliestItem { get; set; }
        public bool IsLatestItem { get; set; }
        public InlineKeyboardMarkup Keyboard { get; set; }
        public string Caption { get; set; }
        public bool HasItems { get; set; }
    }
}