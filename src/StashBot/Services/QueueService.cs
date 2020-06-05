using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using StashBot.Data;
using StashBot.Models;
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
                    double sleepTimeFromConfig = AppSettings.Config_PostInterval;
                    double sleepTime = sleepTimeFromConfig;
                    int queueCount = QueueData.CountQueuedQueueItems();
                    QueueItem soonestQueuedItem = QueueData.GetSoonestQueuedQueueItem();
                    QueueItem latestQueuedItemSinceFirstStart = QueueData.GetLatestPostedQueueItem();
                    bool continueOnFromPreviousStart = true;

                    while (true)
                    {
                        if (queueCount == 0)
                        {
                            continueOnFromPreviousStart = false;
                            Thread.Sleep(60000);
                            queueCount = QueueData.CountQueuedQueueItems();
                        }
                        else
                        {
                            if (continueOnFromPreviousStart)
                            {
                                if (latestQueuedItemSinceFirstStart != null)
                                {
                                    var adjustedSleepTime = (DateTime.UtcNow - latestQueuedItemSinceFirstStart.PostedAt).TotalMilliseconds;
                                    adjustedSleepTime = sleepTime - adjustedSleepTime;

                                    if (adjustedSleepTime >= 0)
                                    {
                                        sleepTime = adjustedSleepTime;
                                    }
                                    else
                                    {
                                        PostQueueItem(soonestQueuedItem);

                                        queueCount = QueueData.CountQueuedQueueItems();
                                        soonestQueuedItem = QueueData.GetSoonestQueuedQueueItem();
                                    }
                                }

                                continueOnFromPreviousStart = false;
                            }
                            else
                            {
                                sleepTime = sleepTimeFromConfig;
                            }

                            Thread.Sleep((int)sleepTime);
                            PostQueueItem(soonestQueuedItem);

                            queueCount = QueueData.CountQueuedQueueItems();
                            soonestQueuedItem = QueueData.GetSoonestQueuedQueueItem();
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageUtilities.PrintErrorMessage(e, Guid.Empty);
                }
            });
        }

        public static void PostQueueItem(QueueItem post)
        {
            if (post != null)
            {
                DateTime postedAt = DateTime.UtcNow;
                var caption = GetQueueCaption(post);
                Message message = null;

                switch (post.Type)
                {
                    case QueueItem.MediaType.Image:
                        message = Program.BotClient.SendPhotoAsync(
                            caption: caption.CaptionText,
                            chatId: AppSettings.Config_ChannelId,
                            parseMode: ParseMode.Html,
                            photo: post.MediaUrl
                        ).Result;
                        break;
                    case QueueItem.MediaType.Video:
                        message = Program.BotClient.SendVideoAsync(
                            caption: caption.CaptionText,
                            chatId: AppSettings.Config_ChannelId,
                            parseMode: ParseMode.Html,
                            video: post.MediaUrl
                        ).Result;
                        break;
                }

                QueueData.SetQueueItemAsPosted(post.Id, postedAt, message.MessageId);
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
                var messageIdString = "";

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

                if(queueItem.MessageId != 0)
                {
                    messageIdString = $"{Environment.NewLine}#️⃣ <b>Message ID:</b> <code>{queueItem.MessageId}</code>";
                }

                string authorNameLink = $"<a href=\"tg://user?id={queueItem.Author.TelegramId}\">{queueItem.Author.TelegramName}</a>";

                string advancedText = $@"—
📩 <b>Poster:</b> {authorNameLink}{statusDateString}
💡 <b>Status:</b> {queueItem.Status}{messageIdString}";

                returnModel.CaptionText += $"{Environment.NewLine}{advancedText}";
            }

            return returnModel;
        }

        public static QueueServiceReturn QueueLink(
            string url,
            TelegramUser user,
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
                InstagramScrapeService _instagramScrapeService = new InstagramScrapeService();
                itemToQueue = _instagramScrapeService.ScrapeInstagramUrl(url, mediaIndex);
            }
            else
            {
                returnModel.Status = QueueServiceReturn.QueueServiceReturnStatus.ServiceNotSupported;
                return returnModel;
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
                    QueueData.AddQueueItem(itemToQueue, user);
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
            return $"{Environment.NewLine}{GeneratorUtilities.GenerateClockEmoji(date)} <b>{status}:</b> {date.ToString("dd-MMM-yy hh:mm:ss zz")}";
        }
    }
}
