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
                try
                {
                    int postsInQueue = QueueData.CountQueuedQueueItems();
                    int sleepTime = AppSettings.Config_PostInterval;

                    while (true)
                    {
                        if (postsInQueue == 0)
                        {
                            Thread.Sleep(60000);
                            postsInQueue = QueueData.CountQueuedQueueItems();
                        }
                        else
                        {
                            Thread.Sleep(sleepTime);
                            PostQueueItem();
                            postsInQueue = QueueData.CountQueuedQueueItems();
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageUtilities.PrintErrorMessage(e, Guid.Empty);
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
            GetQueueCaptionReturn returnModel = new GetQueueCaptionReturn { };

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

            if (!String.IsNullOrEmpty(queueItem.Name))
            {
                if (!String.IsNullOrEmpty(queueItem.UsernameUrl))
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

            if (!String.IsNullOrEmpty(queueItem.SourceUrl))
            {
                string sourceName = String.IsNullOrEmpty(queueItem.SourceName) ? "source" : queueItem.SourceName;

                creditText += " | ";
                sourceText += $"<a href=\"{queueItem.SourceUrl}\">🔗 {sourceName}</a>";
            }

            returnModel.CaptionText = $"{mediaTypeText}{creditText}{sourceText}";

            if (advanced)
            {
                var statusDateString = "";

                switch (queueItem.Status)
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

        public static QueueServiceReturn QueueLink(
            string url,
            TelegramUser author,
            int mediaIndex = 0,
            string customName = ""
        )
        {
            QueueServiceReturn returnModel = new QueueServiceReturn { };
            QueueItem itemToQueue = null;

            if (
                url.StartsWith("https://mobile.twitter.com") ||
                url.StartsWith("https://twitter.com")
            )
            {
                TwitterScrapeService _twitterScrapeService = new TwitterScrapeService();
                itemToQueue = _twitterScrapeService.ScrapeTwitterUrl(url, mediaIndex, customName);
            }
            else if (
                url.StartsWith("https://instagram.com") ||
                url.StartsWith("https://www.instagram.com")
            )
            {
                InstagrmScrapeService _instagramScrapeService = new InstagrmScrapeService();
                itemToQueue = _instagramScrapeService.ScrapeInstagramUrl(url);
            }

            if (itemToQueue != null)
            {
                var duplicate = QueueData.GetQueueItemBySourceUrl(itemToQueue.MediaUrl);

                if (duplicate != null)
                {
                    returnModel.Status = QueueServiceReturn.QueueServiceReturnStatus.Duplicate;
                }
                else
                {
                    itemToQueue.AuthorTelegramId = author.Id;
                    itemToQueue.AuthorTelegramName = author.Name;
                    itemToQueue.AuthorTelegramUsername = author.Username;

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

        public static void RemoveQueueItem(
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