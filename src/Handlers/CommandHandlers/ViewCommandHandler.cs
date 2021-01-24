using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using GearstashBot.Data;
using GearstashBot.Exceptions;
using GearstashBot.I18n;
using GearstashBot.Models;
using GearstashBot.Models.ArgumentModels;
using GearstashBot.Models.ReturnModels.ViewCommandHandlerReturns;
using GearstashBot.Services;
using GearstashBot.Utilities;

namespace GearstashBot.Handlers.CommandHandlers
{
    public class ViewCommandHandler
    {
        private static string PlaceholderImage = "https://i.imgur.com/PmzeWAH.png";

        public static void Invoke(CommandHandlerArguments arguments)
        {
            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                var initMessage = Program.BotClient.SendPhotoAsync(
                        caption: $"<i>{Localization.GetPhrase(Localization.Phrase.LoadingQueue, arguments.TelegramUser)}</i>",
                        chatId: arguments.TelegramMessageEvent.Message.Chat.Id,
                        parseMode: ParseMode.Html,
                        photo: PlaceholderImage
                    ).Result;

                arguments.TelegramMessageEvent.Message.MessageId = initMessage.MessageId;

                if (arguments.CommandArguments != null)
                {
                    // TODO: Find by source URL?
                    if (arguments.CommandArguments.Length == 1)
                    {
                        Regex tMeIdRegex = new Regex(@"^(https:\/\/t.me\/)(\w+)([\/]{1})([0-9]*)$");
                        Match parsedTMeId = tMeIdRegex.Match(arguments.CommandArguments[0]);

                        if (parsedTMeId.Success)
                        {
                            int messageId = Convert.ToInt32(parsedTMeId.Groups[4].Value);

                            NavigateToMessageId(
                                arguments.TelegramMessageEvent.Message.Chat.Id,
                                arguments.TelegramMessageEvent.Message.MessageId,
                                arguments.TelegramUser,
                                messageId
                            );
                        }
                        else
                        {
                            Program.BotClient.DeleteMessageAsync(
                                chatId: arguments.TelegramMessageEvent.Message.Chat.Id,
                                messageId: arguments.TelegramMessageEvent.Message.MessageId
                            );

                            throw new ArgumentException();
                        }
                    }
                }
                else
                {
                    NavigateTo(
                        arguments.TelegramMessageEvent.Message.Chat.Id,
                        arguments.TelegramMessageEvent.Message.MessageId,
                        arguments.TelegramUser
                    );
                }
            }
            else
            {
                throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.NoPermissionViewQueue, arguments.TelegramUser));
            }
        }

        public static async Task InvokeChange(CommandHandlerArguments arguments)
        {
            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                await NavigateTo(
                    arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId,
                    arguments.TelegramUser,
                    arguments.CommandArguments[1].ToString(),
                    Convert.ToInt32(arguments.CommandArguments[0])
                );
            }
            else
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                );

                throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.NoPermissionViewQueue, arguments.TelegramUser));
            }

            await Program.BotClient.AnswerCallbackQueryAsync(
                callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id
            );
        }

        public static async Task InvokeDelete(CommandHandlerArguments arguments)
        {
            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                if (!Constants.IsPostingDisabled)
                {
                    var queueItemsData = GetQueueItemsData(
                        arguments.TelegramUser,
                        GetQueueItemStatusEnum(arguments.CommandArguments[1]),
                        Convert.ToInt32(arguments.CommandArguments[0]));
                    var authorId = arguments.TelegramUser.Id;

                    bool allowedToDeleteThisQueueItem = false;

                    var statusText = "";

                    if (AuthorData.CanAuthorQueue(authorId))
                    {
                        if (queueItemsData.SelectedQueuedItem.Author.TelegramId == authorId)
                        {
                            allowedToDeleteThisQueueItem = true;
                        }
                        else
                        {
                            if (AuthorData.CanAuthorDeleteOthers(authorId))
                            {
                                allowedToDeleteThisQueueItem = true;
                            }
                        }
                    }

                    if (allowedToDeleteThisQueueItem)
                    {
                        statusText = MessageUtilities.CreateSuccessMessage(Localization.GetPhrase(Localization.Phrase.Deleted, arguments.TelegramUser));

                        if (queueItemsData.SelectedQueuedItem.Status == QueueItem.QueueStatus.Posted)
                        {
                            try
                            {

                                if (queueItemsData.SelectedQueuedItem.MessageId != 0)
                                {
                                    await Program.BotClient.DeleteMessageAsync(
                                        chatId: Models.AppSettings.Config_ChannelId,
                                        messageId: Convert.ToInt32(queueItemsData.SelectedQueuedItem.MessageId)
                                    );
                                    statusText = MessageUtilities.CreateSuccessMessage(
                                        Localization.GetPhrase(
                                            Localization.Phrase.DeletedXFromChannel,
                                            arguments.TelegramUser,
                                            new string[] {
                                        queueItemsData.SelectedQueuedItem.MessageId.ToString()
                                            }
                                        )
                                    );
                                }
                                else
                                {
                                    statusText = MessageUtilities.CreateWarningMessage(
                                        Localization.GetPhrase(
                                            Localization.Phrase.CannotDeleteXFromChannel,
                                            arguments.TelegramUser,
                                            new string[] {
                                        queueItemsData.SelectedQueuedItem.MessageId.ToString()
                                            }
                                        )
                                    );
                                }
                            }
                            catch (Exception e)
                            {
                                MessageUtilities.PrintErrorMessage(e, Guid.Empty);
                                statusText = MessageUtilities.CreateWarningMessage(
                                    Localization.GetPhrase(
                                        Localization.Phrase.CannotDeleteXFromChannel,
                                        arguments.TelegramUser,
                                        new string[] {
                                    queueItemsData.SelectedQueuedItem.MessageId.ToString()
                                        }
                                    )
                                );
                            }
                        }
                    }
                    else
                    {
                        statusText = MessageUtilities.CreateWarningMessage(Localization.GetPhrase(Localization.Phrase.NoPermissionRemovePost, arguments.TelegramUser));
                    }

                    await Program.BotClient.AnswerCallbackQueryAsync(
                        callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id,
                        text: statusText
                    );

                    if (allowedToDeleteThisQueueItem)
                    {
                        bool purge = (queueItemsData.SelectedQueuedItem.Status == QueueItem.QueueStatus.Deleted) ? true : false;
                        QueueService.RemoveQueueItem(queueItemsData.SelectedQueuedItem.Id);

                        int idToNavigateTo = queueItemsData.PreviousQueuedItem.Id;

                        if (queueItemsData.IsEarliestItem)
                        {
                            idToNavigateTo = queueItemsData.NextQueuedItem.Id;
                        }

                        await NavigateTo(
                            arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                            arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId,
                            arguments.TelegramUser,
                            arguments.CommandArguments[1],
                            idToNavigateTo
                        );
                    }
                }
                else
                {
                    await Program.BotClient.AnswerCallbackQueryAsync(
                        callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id,
                        text: MessageUtilities.CreateWarningMessage(
                            Localization.GetPhrase(Localization.Phrase.CannotDeleteTemporarilyDueToLongRunningRequest, arguments.TelegramUser)
                        )
                    );
                }
            }
            else
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                );

                throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.NoPermissionViewQueue, arguments.TelegramUser));
            }
        }

        public static async Task InvokeList(CommandHandlerArguments arguments)
        {
            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                await Program.BotClient.EditMessageReplyMarkupAsync(
                    chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId,
                    replyMarkup: GenerateListKeyboard(
                        GetQueueItemStatusEnum(arguments.CommandArguments[1]),
                        Convert.ToInt32(arguments.CommandArguments[0]),
                        arguments.TelegramUser
                    )
                );
            }
            else
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                );

                throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.NoPermissionViewQueue, arguments.TelegramUser));
            }
        }

        private static InlineKeyboardMarkup GenerateDefaultKeyboard(
            int currentId,
            int previousId,
            int nextId,
            bool isEarliestItem,
            bool isLatestItem,
            QueueItem.QueueStatus status,
            TelegramUser user
        )
        {
            bool isLookingAtPosted = (status == QueueItem.QueueStatus.Posted) ? true : false;
            bool isLookingAtQueued = (status == QueueItem.QueueStatus.Queued) ? true : false;

            string currentlyViewingString = GetStringFromStatus(status, user);

            return new InlineKeyboardMarkup(new[]
            {
                GenerateNavigationKeyboardButtons(
                    status,
                    previousId,
                    nextId,
                    isEarliestItem,
                    isLatestItem,
                    user
                ),
                GenerateActionKeyboardButtons(
                    status,
                    currentId,
                    user
                ),
                new[]
                {
                    InlineKeyboardButton.WithCallbackData($"👀 {Localization.GetPhrase(Localization.Phrase.Viewing, user)}: {currentlyViewingString}", $"view_list:{currentId}:{status.ToString()}")
                }
            });
        }

        private static InlineKeyboardMarkup GenerateListKeyboard(
            QueueItem.QueueStatus status,
            int currentId,
            TelegramUser user,
            bool emptyQueue = false
        )
        {
            string exitButtonString = emptyQueue ?
                $"🔄 {Localization.GetPhrase(Localization.Phrase.Retry, user)}" :
                $"◀️ {Localization.GetPhrase(Localization.Phrase.Back, user)}";

            return new InlineKeyboardMarkup(new[]
            {
                new[] {
                    InlineKeyboardButton.WithCallbackData($"📤 {Localization.GetPhrase(Localization.Phrase.Queued, user)}", "view_nav:0:Queued"),
                    InlineKeyboardButton.WithCallbackData($"📥 {Localization.GetPhrase(Localization.Phrase.Posted, user)}", "view_nav:0:Posted")
                },
                new[] {
                    InlineKeyboardButton.WithCallbackData($"{exitButtonString}", $"view_nav:{currentId}:{status.ToString()}")
                }
            });
        }

        private static InlineKeyboardButton[] GenerateActionKeyboardButtons(
            QueueItem.QueueStatus status,
            int currentId,
            TelegramUser user
        )
        {
            var deleteString = Localization.GetPhrase(Localization.Phrase.Delete, user);

            switch (status)
            {
                default:
                    return new[]
                    {
                        InlineKeyboardButton.WithCallbackData($"🗑 {deleteString}", $"view_del:{currentId}:{status.ToString()}")
                    };
            }
        }

        private static InlineKeyboardButton[] GenerateNavigationKeyboardButtons(
            QueueItem.QueueStatus status,
            int previousId,
            int nextId,
            bool isEarliestItem,
            bool isLatestItem,
            TelegramUser user
        )
        {
            var statusText = status.ToString();

            var nextString = Localization.GetPhrase(Localization.Phrase.Later, user);
            var lastString = Localization.GetPhrase(Localization.Phrase.Latest, user);
            var previousString = Localization.GetPhrase(Localization.Phrase.Sooner, user);
            var firstString = Localization.GetPhrase(Localization.Phrase.Soonest, user);

            if (status == QueueItem.QueueStatus.Posted)
            {
                nextString = Localization.GetPhrase(Localization.Phrase.Earlier, user);
                lastString = Localization.GetPhrase(Localization.Phrase.Earliest, user);
                previousString = Localization.GetPhrase(Localization.Phrase.Later, user);
                firstString = Localization.GetPhrase(Localization.Phrase.Latest, user);
            }

            string earlierButton = isEarliestItem ? $"{lastString} ⏩" : $"⬅️ {previousString}";
            string laterButton = isLatestItem ? $"⏪ {firstString}" : $"{nextString} ➡️";

            return new[]
            {
                InlineKeyboardButton.WithCallbackData(earlierButton, $"view_nav:{previousId.ToString()}:{statusText}"),
                InlineKeyboardButton.WithCallbackData(laterButton, $"view_nav:{nextId.ToString()}:{statusText}"),
            };
        }

        private static GetQueueItemsDataReturn GetQueueItemsData(TelegramUser user, QueueItem.QueueStatus status, int id = 0)
        {
            GetQueueItemsDataReturn returnModel = new GetQueueItemsDataReturn { };

            List<QueueItem> queue = null;

            switch (status)
            {
                case QueueItem.QueueStatus.Posted:
                    queue = QueueData.ListPostedQueueItems();
                    break;
                case QueueItem.QueueStatus.Queued:
                default:
                    queue = QueueData.ListQueuedQueueItems();
                    break;
            }

            if (queue.Count() > 0)
            {
                // TODO: Handle item that is missing various data

                QueueItem selectedQueuedItem = (id == 0) ? queue[0] : queue.Where(q => q.Id == id).FirstOrDefault();
                selectedQueuedItem = (selectedQueuedItem == null) ? queue[0] : selectedQueuedItem;

                int minIndex = 0;
                int maxIndex = queue.Count() - 1;

                int currentIndex = queue.IndexOf(selectedQueuedItem);
                int previousIndex = currentIndex - 1;
                int nextIndex = currentIndex + 1;

                QueueItem previousQueuedItem = (previousIndex < minIndex) ? queue[maxIndex] : queue[previousIndex];
                QueueItem nextQueuedItem = (nextIndex > maxIndex) ? queue[minIndex] : queue[nextIndex];

                bool isEarliestItem = (previousIndex < minIndex);
                bool isLatestItem = (nextIndex > maxIndex);

                var caption = QueueService.GetQueueCaption(selectedQueuedItem, true, user).CaptionText;

                var keyboard = GenerateDefaultKeyboard(
                    selectedQueuedItem.Id,
                    previousQueuedItem.Id,
                    nextQueuedItem.Id,
                    isEarliestItem,
                    isLatestItem,
                    status,
                    user
                );

                returnModel.SelectedQueuedItem = selectedQueuedItem;
                returnModel.PreviousQueuedItem = previousQueuedItem;
                returnModel.NextQueuedItem = nextQueuedItem;
                returnModel.IsEarliestItem = isEarliestItem;
                returnModel.IsLatestItem = isLatestItem;
                returnModel.Keyboard = keyboard;
                returnModel.Caption = caption;
                returnModel.HasItems = true;
            }
            else
            {
                returnModel.HasItems = false;
            }

            return returnModel;
        }

        private static QueueItem.QueueStatus GetQueueItemStatusEnum(string toParse)
        {
            QueueItem.QueueStatus status = QueueItem.QueueStatus.Queued;
            Enum.TryParse(toParse, out status);
            return status;
        }

        private static string GetStringFromStatus(
            QueueItem.QueueStatus status,
            TelegramUser user
        )
        {
            Localization.Phrase phrase = Localization.Phrase.Queued;

            switch (status)
            {
                case QueueItem.QueueStatus.Deleted:
                    phrase = Localization.Phrase.Deleted;
                    break;
                case QueueItem.QueueStatus.Posted:
                    phrase = Localization.Phrase.Posted;
                    break;
                case QueueItem.QueueStatus.Queued:
                    phrase = Localization.Phrase.Queued;
                    break;
            }

            return Localization.GetPhrase(phrase, user);
        }

        private static async Task HandleEmptyQueue(
            long chatId,
            int messageId,
            TelegramUser user,
            QueueItem.QueueStatus status)
        {
            await Program.BotClient.EditMessageMediaAsync(
                chatId: chatId,
                media: new InputMediaPhoto
                {
                    Media = PlaceholderImage
                },
                messageId: messageId
            );

            var caption = "";

            switch (status)
            {
                case QueueItem.QueueStatus.Deleted:
                    caption = Localization.GetPhrase(Localization.Phrase.NothingIsDeleted, user);
                    break;
                case QueueItem.QueueStatus.Posted:
                    caption = Localization.GetPhrase(Localization.Phrase.NothingIsPosted, user);
                    break;
                case QueueItem.QueueStatus.Queued:
                default:
                    caption = Localization.GetPhrase(Localization.Phrase.NothingIsQueued, user);
                    break;
            }

            await Program.BotClient.EditMessageCaptionAsync(
                caption: MessageUtilities.CreateWarningMessage(caption),
                chatId: chatId,
                messageId: messageId,
                replyMarkup: GenerateListKeyboard(status, 0, user, true),
                parseMode: ParseMode.Html
            );
        }

        private static async Task NavigateTo(
            long chatId,
            int messageId,
            TelegramUser user,
            string statusText = "Queued",
            int queueItemId = 0
        )
        {
            QueueItem.QueueStatus status = GetQueueItemStatusEnum(statusText);

            var queueItemsData = GetQueueItemsData(
                    user,
                    status,
                    queueItemId);

            if (queueItemsData.HasItems)
            {
                var selectedQueuedItem = queueItemsData.SelectedQueuedItem;
                var controlKeyboard = queueItemsData.Keyboard;
                var caption = queueItemsData.Caption;

                try
                {
                    switch (selectedQueuedItem.Type)
                    {
                        case QueueItem.MediaType.Image:
                            await Program.BotClient.EditMessageMediaAsync(
                                chatId: chatId,
                                media: new InputMediaPhoto
                                {
                                    Media = selectedQueuedItem.MediaUrl
                                },
                                messageId: messageId
                            );
                            break;
                        case QueueItem.MediaType.Video:
                            await Program.BotClient.EditMessageMediaAsync(
                                chatId: chatId,
                                media: new InputMediaVideo
                                {
                                    Media = selectedQueuedItem.MediaUrl
                                },
                                messageId: messageId
                            );
                            break;
                    }
                }
                catch (Exception e)
                {
                    switch (e.Message)
                    {
                        case "Bad Request: MEDIA_EMPTY":
                        case "Bad Request: WEBPAGE_CURL_FAILED":
                            await Program.BotClient.EditMessageMediaAsync(
                                chatId: chatId,
                                media: new InputMediaPhoto
                                {
                                    Media = PlaceholderImage
                                },
                                messageId: messageId
                            );

                            string errorMessage = Localization.GetPhrase(
                                Localization.Phrase.CannotFetchFile,
                                user,
                                new string[] {
                                    selectedQueuedItem.MediaUrl,
                                    e.Message.Replace("Bad Request: ", "")
                                }
                            );

                            caption = MessageUtilities.CreateWarningMessage($"{errorMessage}{Environment.NewLine}—{Environment.NewLine}") + caption;
                            break;
                        default:
                            throw;
                    }
                }

                await Program.BotClient.EditMessageCaptionAsync(
                    caption: caption,
                    chatId: chatId,
                    messageId: messageId,
                    replyMarkup: controlKeyboard,
                    parseMode: ParseMode.Html
                );
            }
            else
            {
                await HandleEmptyQueue(chatId, messageId, user, status);
            }
        }

        private static async Task NavigateToMessageId(
            long chatId,
            int messageId,
            TelegramUser user,
            int channelMessageId
        )
        {
            var queueItem = QueueData.GetQueueItemByMessageId(channelMessageId);

            if (queueItem != null && queueItem.Status == QueueItem.QueueStatus.Posted)
            {
                await NavigateTo(
                    chatId,
                    messageId,
                    user,
                    "Posted",
                    queueItem.Id
                );
            }
            else
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: chatId,
                    messageId: messageId
                );

                MessageUtilities.SendWarningMessage(
                    Localization.GetPhrase(
                        Localization.Phrase.CannotFindMessageX,
                        user,
                        new string[] {
                            channelMessageId.ToString()
                        }
                    ),
                    chatId);
            }
        }
    }
}