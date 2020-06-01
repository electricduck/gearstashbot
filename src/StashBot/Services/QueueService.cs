using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StashBot.Data;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Models.ReturnModels.QueueServiceReturns;
using StashBot.Models.ReturnModels.ServiceReturnModels;
using StashBot.Services.ScrapeServices;
using StashBot.Utilities;

namespace StashBot.Services
{
    public class QueueService
    {
        public static void PollQueue()
        {
            Task.Run(() =>
            {
                int postsInQueue = 0;
                int sleepTime = AppSettings.Config_PostInterval;

                while (true)
                {
                    Thread.Sleep(sleepTime);
                    postsInQueue = QueueData.CountQueuedQueueItems();
                    PostQueueItem();
                }
            });
        }

        public static void PostQueueItem()
        {
            var post = QueueData.GetFirstQueueItem();

            if (post != null)
            {
                DateTime postedAt = DateTime.UtcNow;
                var caption = GetQueueCaption(post);

                switch (post.Type)
                {
                    case QueueItem.MediaType.Image:
                        SendPhotoArguments photoMessage = new SendPhotoArguments
                        {
                            Caption = caption.CaptionText,
                            ChatId = AppSettings.Config_ChannelId,
                            Photo = post.MediaUrl
                        };
                        TelegramApiService.SendPhoto(photoMessage, Program.BotClient, null);
                        break;
                    case QueueItem.MediaType.Video:
                        SendVideoArguments videoMessage = new SendVideoArguments
                        {
                            Caption = caption.CaptionText,
                            ChatId = AppSettings.Config_ChannelId,
                            Video = post.MediaUrl
                        };
                        TelegramApiService.SendVideo(videoMessage, Program.BotClient, null);
                        break;
                }

                QueueData.SetQueueItemAsPosted(post.Id, postedAt);

                MessageUtilities.PrintSuccessMessage($"Posted #{post.Id} at {postedAt.ToString("yyyy-MM-dd hh:mm:ss zzz")}");
            }
        }

        public static GetQueueCaptionReturn GetQueueCaption(
            QueueItem queueItem,
            bool advanced = false
        )
        {
            GetQueueCaptionReturn returnModel = new GetQueueCaptionReturn {};

            string creditText = "";
            string mediaTypeText = "";
            string sourceText = "";

            switch (queueItem.Type)
            {
                case QueueItem.MediaType.Image:
                    mediaTypeText = "📷 | ";
                    break;
                case QueueItem.MediaType.Video:
                    mediaTypeText = "📹 | ";
                    break;
            }

            if(!String.IsNullOrEmpty(queueItem.Name))
            {
                if(!String.IsNullOrEmpty(queueItem.UsernameUrl))
                {
                    creditText += $"👤 <a href=\"{queueItem.UsernameUrl}\">{queueItem.Name}</a>";
                }
                else
                {
                    creditText += $"👤 <i>{queueItem.Name}</i>";
                }
            }
            else
            {
                creditText += $"👤 <i>(unknown)</i>";
            }

            if(!String.IsNullOrEmpty(queueItem.SourceUrl))
            {
                string sourceName = String.IsNullOrEmpty(queueItem.SourceName) ? "source" : queueItem.SourceName;

                creditText += " | ";
                sourceText += $"<a href=\"{queueItem.SourceUrl}\">🔗 {sourceName}</a>";
            }

            returnModel.CaptionText = $"{mediaTypeText}{creditText}{sourceText}";

            if(advanced)
            {
                var statusDateString = "";

                switch(queueItem.Status)
                {
                    case QueueItem.QueueStatus.Queued:
                        statusDateString = GenerateStatusDateStringForAdvancedCaption(queueItem.QueuedAt, QueueItem.QueueStatus.Queued);
                        break;
                    case QueueItem.QueueStatus.Deleted:
                        statusDateString = GenerateStatusDateStringForAdvancedCaption(queueItem.DeletedAt, QueueItem.QueueStatus.Deleted);
                        break;
                    case QueueItem.QueueStatus.Posted:
                        statusDateString = GenerateStatusDateStringForAdvancedCaption(queueItem.PostedAt, QueueItem.QueueStatus.Posted);
                        break;
                }

                string authorNameLink = $"<a href=\"tg://user?id={queueItem.AuthorTelegramId}\">{queueItem.AuthorTelegramName}</a>";

                string advancedText = $@"—
#️⃣ <b>ID:</b> {queueItem.Id}
✍️ <b>Author:</b> {authorNameLink}{statusDateString}
💡 <b>Status:</b> {queueItem.Status}";

                returnModel.CaptionText += $"{Environment.NewLine}{advancedText}";
            }

            return returnModel;
        }

        public static QueueServiceReturn QueueLinkPost(
            string url,
            int authorId,
            string authorName,
            string authorUsername,
            int mediaIndex = 0,
            string name = ""
        )
        {
            QueueServiceReturn returnModel = new QueueServiceReturn { };
            QueueItem itemToQueue = null;

            if (url.StartsWith("https://twitter.com") || url.StartsWith("https://mobile.twitter"))
            {
                TwitterScrapeService _twitterScrapeService = new TwitterScrapeService();

                itemToQueue = _twitterScrapeService.ScrapeTwitterUrl(
                    url,
                    mediaIndex,
                    name
                );
            }

            if (itemToQueue != null)
            {
                var duplicate = QueueData.GetQueueItemBySourceUrl(itemToQueue.SourceUrl);

                if (duplicate != null)
                {
                    returnModel.Status = QueueServiceReturn.QueueServiceReturnStatus.Duplicate;
                }
                else
                {
                    itemToQueue.AuthorTelegramId = authorId;
                    itemToQueue.AuthorTelegramName = authorName;
                    itemToQueue.AuthorTelegramUsername = authorUsername;
                    QueueData.AddQueueItem(itemToQueue);
                    returnModel.Status = QueueServiceReturn.QueueServiceReturnStatus.Queued;
                }
            }
            else
            {
                returnModel.Status = QueueServiceReturn.QueueServiceReturnStatus.SourceUrlNotFound;
            }

            return returnModel;
        }

        public static void RemoveItem(
            int id
        )
        {
            QueueData.DeleteQueueItem(id, false);
        }

        private static string GenerateStatusDateStringForAdvancedCaption(DateTime date, Enum status)
        {
            return $"{Environment.NewLine}{GeneratorUtilities.GenerateClockEmoji(date)} <b>{status}:</b> <code>{date.ToString("dd-MMM-yy hh:mm:ss zz")}</code>";
        }
    }
}