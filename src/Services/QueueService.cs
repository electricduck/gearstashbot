using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using GearstashBot.Data;
using GearstashBot.I18n;
using GearstashBot.Models;
using GearstashBot.Models.ReturnModels.QueueServiceReturns;
using GearstashBot.Models.ReturnModels.ServiceReturnModels;
using GearstashBot.Utilities;

namespace GearstashBot.Services
{
    public class QueueService
    {
        public static void PollQueue()
        {
            Task.Run(() =>
            {
                try
                {
                    var postLagSW = new Stopwatch();

                    double sleepTimeFromConfig = AppSettings.Config_PostInterval;
                    double sleepTime = 0;
                    int queueCount = QueueData.CountQueuedQueueItems();
                    QueueItem soonestQueuedItem = null;
                    QueueItem latestPostedItem = QueueData.GetLatestPostedQueueItem();
                    bool continueOnFromPreviousStart = true;

                    while (true)
                    {
                        if (queueCount == 0)
                        {
                            continueOnFromPreviousStart = false;
                            Thread.Sleep(10000); // 10 seconds
                            queueCount = QueueData.CountQueuedQueueItems();
                        }
                        else
                        {
                            postLagSW.Start();

                            if (continueOnFromPreviousStart)
                            {
                                if (latestPostedItem != null)
                                {
                                    var adjustedSleepTime = sleepTimeFromConfig - (DateTime.UtcNow - latestPostedItem.PostedAt).TotalMilliseconds;

                                    if (adjustedSleepTime >= 0)
                                    {
                                        sleepTime = adjustedSleepTime;
                                    }
                                    else
                                    {
                                        sleepTime = 0;
                                    }
                                }
                                else
                                {
                                    sleepTime = sleepTimeFromConfig;
                                }

                                continueOnFromPreviousStart = false;
                            }

                            Thread.Sleep((int)sleepTime);
                            soonestQueuedItem = QueueData.GetSoonestQueuedQueueItem();
                            bool wasPosted = PostQueueItem(soonestQueuedItem);

                            if (wasPosted)
                            {
                                sleepTime = sleepTimeFromConfig;
                            }
                            else
                            {
                                sleepTime = 0;
                            }

                            queueCount = QueueData.CountQueuedQueueItems();
                            Constants.PostLag = postLagSW.Elapsed;
                        }

                        postLagSW.Stop();
                        postLagSW.Reset();
                    }
                }
                catch (Exception e)
                {
                    MessageUtilities.PrintErrorMessage(e, Guid.Empty);
                }
            });
        }

        public static bool PostQueueItem(QueueItem post)
        {
            if (post != null)
            {
                DateTime postedAt = DateTime.UtcNow;
                var caption = GetQueueCaption(post);
                Message message = null;
                bool failed = false;
                string failureReason = "";

                try
                {
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
                }
                catch (Exception e)
                {
                    string errorMessage = e.Message;

                    if (
                        errorMessage.Contains("Bad Request:") ||
                        errorMessage.Contains("chat not found")
                    )
                    {
                        failed = true;
                        failureReason = e.Message;
                    }
                    else
                    {
                        throw;
                    }
                }

                if (!failed)
                {
                    QueueData.SetQueueItemAsPosted(post.Id, postedAt, message.MessageId);
                    MessageUtilities.PrintSuccessMessage($"Posted #{post.Id} at {postedAt.ToString("yyyy-MM-dd HH:mm:ss zzz")}");
                    return true;
                }
                else
                {
                    QueueData.SetQueueItemAsPostFailed(post.Id, postedAt, failureReason);
                    MessageUtilities.PrintWarningMessage($"Unable to post #{post.Id}: {failureReason}");
                    return false;
                }
            }

            return false;
        }

        public static QueueServiceReturn QueueLink(
            string url,
            TelegramUser user,
            int mediaIndex = 0,
            string customName = ""
        )
        {
            QueueServiceReturn returnModel = new QueueServiceReturn { };
            ScrapeService scrapeService = new ScrapeService();
            QueueItem itemToQueue = null;

            Scrape scrape = scrapeService.Scrape(url);

            if (scrape != null)
            {
                if (scrape.HasMedia)
                {
                    if (url.Contains("/photo/") && scrape.SourceName == "Twitter") // NOTE: This will cause issues for the Twitter account @photo
                    {
                        mediaIndex = Convert.ToInt32(url.Substring(url.IndexOf("/photo/") + "/photo/".Length)) - 1;
                    }

                    Media selectedMedia = (mediaIndex + 1 > scrape.Media.Count || mediaIndex < 0) ?
                        scrape.Media[0] :
                        scrape.Media[mediaIndex];

                    itemToQueue = new QueueItem
                    {
                        MediaUrl = selectedMedia.MediaUrl,
                        Name = scrape.Name,
                        SourceName = scrape.SourceName,
                        SourceUrl = selectedMedia.SourceUrl,
                        Type = selectedMedia.Type,
                        UsernameUrl = scrape.UsernameUrl
                    };
                }
                else
                {
                    returnModel.Status = QueueServiceReturn.QueueServiceReturnStatus.SourceUrlNotFound;
                }
            }
            else
            {
                // TODO: Handle this better?
                returnModel.Status = QueueServiceReturn.QueueServiceReturnStatus.ServiceNotSupported;
            }

            if (itemToQueue != null)
            {
                var duplicate = QueueData.GetQueueItemBySourceUrl(itemToQueue.SourceUrl);
                returnModel.Status = QueueServiceReturn.QueueServiceReturnStatus.Queued;

                if (duplicate != null)
                {
                    if (AppSettings.Config_WarnOnDuplicate)
                        returnModel.Status = QueueServiceReturn.QueueServiceReturnStatus.Duplicate;
                }
                else
                {
                    QueueData.AddQueueItem(itemToQueue, user);
                }
            }

            return returnModel;
        }

        public static void RemoveQueueItem(
            int id
        )
        {
            QueueData.DeleteQueueItem(id, true);
        }

        public static GetQueueCaptionReturn GetQueueCaption(
            QueueItem queueItem,
            bool advanced = false,
            TelegramUser user = null
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
                //DateTime scheduledDate = queueItem.QueuedAt // what the fuck r u doin
                //    .AddMilliseconds(AppSettings.Config_PostInterval)
                //    .AddMilliseconds(Constants.PostLag.Milliseconds);

                string statusDateString = "";
                string scheduledDateString = "";
                string messageIdString = "";

                switch (queueItem.Status)
                {
                    case QueueItem.QueueStatus.Queued:
                        //scheduledDateString = GenerateDateStringForAdvancedCaption(scheduledDate, "Scheduled");
                        statusDateString = GenerateDateStringForAdvancedCaption(queueItem.QueuedAt, Localization.GetPhrase(Localization.Phrase.Queued, user));
                        break;
                    case QueueItem.QueueStatus.Deleted:
                        statusDateString = GenerateDateStringForAdvancedCaption(queueItem.DeletedAt, "Deleted");
                        break;
                    case QueueItem.QueueStatus.Posted:
                        statusDateString = GenerateDateStringForAdvancedCaption(queueItem.PostedAt, Localization.GetPhrase(Localization.Phrase.Posted, user));
                        break;
                }

                if (queueItem.Status == QueueItem.QueueStatus.Posted)
                {
                    messageIdString = $"{Environment.NewLine}#️⃣ <b>{Localization.GetPhrase(Localization.Phrase.MessageID, user)}:</b> ";

                    if (queueItem.MessageId != 0)
                    {
                        messageIdString += $"<code>{queueItem.MessageId}</code>";
                    }
                    else
                    {
                        messageIdString += $"<i>({Localization.GetPhrase(Localization.Phrase.NotSet, user)})</i>";
                    }
                }

                string authorNameLink = $"<a href=\"tg://user?id={queueItem.Author.TelegramId}\">{queueItem.Author.TelegramName}</a>";

                string advancedText = $@"—
📩 <b>{Localization.GetPhrase(Localization.Phrase.Author, user)}:</b> {authorNameLink}{statusDateString}{scheduledDateString}{messageIdString}";

                returnModel.CaptionText += $"{Environment.NewLine}{advancedText}";
            }

            return returnModel;
        }

        private static string GenerateDateStringForAdvancedCaption(DateTime date, string statusText)
        {
            return $"{Environment.NewLine}{GeneratorUtilities.GenerateClockEmoji(date)} <b>{statusText}:</b> <code>{date.ToString("dd-MMM-yy HH:mm:ss zz")}</code>";
        }
    }
}
