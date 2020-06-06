using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using StashBot.Data;
using StashBot.Exceptions;
using StashBot.I18n;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Models.ReturnModels.ViewCommandHandlerReturns;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class ViewCommandHandler
    {
        private static string placeholderImage = "https://i.imgur.com/PmzeWAH.png";

        public static Help Help = new Help
        {
            Command = "view",
            Description = "View and manage posts in queue"
        };

        public static void Invoke(CommandHandlerArguments arguments)
        {
            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                var initMessage = Program.BotClient.SendPhotoAsync(
                    caption: "<i>Loading Queue...</i>",
                    chatId: arguments.TelegramMessageEvent.Message.Chat.Id,
                    parseMode: ParseMode.Html,
                    photo: placeholderImage
                ).Result;

                arguments.TelegramMessageEvent.Message.MessageId = initMessage.MessageId;
                NavigateTo(
                    arguments.TelegramMessageEvent.Message.Chat.Id,
                    arguments.TelegramMessageEvent.Message.MessageId,
                    arguments.TelegramUser
                );
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
            var queueItemsData = GetQueueItemsData(
                arguments.TelegramUser,
                GetQueueItemStatusEnum(arguments.CommandArguments[1]),
                Convert.ToInt32(arguments.CommandArguments[0]));
            var authorId = arguments.TelegramUser.Id;

            bool allowedToDeleteThisQueueItem = false;
            bool deleted = true;

            var statusText = MessageUtilities.CreateWarningMessage(Localization.GetPhrase(Localization.Phrase.NoPermissionRemovePost, arguments.TelegramUser));

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
                statusText = MessageUtilities.CreateSuccessMessage(
                    Localization.GetPhrase(Localization.Phrase.DeletedFromQueue, arguments.TelegramUser)
                );

                try
                {
                    if (queueItemsData.SelectedQueuedItem.Status == QueueItem.QueueStatus.Posted)
                    {
                        if (queueItemsData.SelectedQueuedItem.MessageId != 0)
                        {
                            await Program.BotClient.DeleteMessageAsync(
                                chatId: Models.AppSettings.Config_ChannelId,
                                messageId: Convert.ToInt32(queueItemsData.SelectedQueuedItem.MessageId)
                            );
                            statusText = MessageUtilities.CreateSuccessMessage($"Deleted #{queueItemsData.SelectedQueuedItem.MessageId} from channel");
                        }
                    } else {
                        statusText = MessageUtilities.CreateWarningMessage($"Unable to delete #{queueItemsData.SelectedQueuedItem.MessageId} from channel");
                        deleted = false;
                    }
                }
                catch (Exception)
                {
                    statusText = MessageUtilities.CreateWarningMessage($"Unable to delete #{queueItemsData.SelectedQueuedItem.MessageId} from channel");
                    deleted = false;
                }

                if(deleted) {
                    QueueService.RemoveQueueItem(queueItemsData.SelectedQueuedItem.Id);
                }
            }
            else
            {
                statusText = MessageUtilities.CreateWarningMessage(
                    Localization.GetPhrase(Localization.Phrase.NoPermissionRemovePost, arguments.TelegramUser)
                );
            }

            await Program.BotClient.AnswerCallbackQueryAsync(
                callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id,
                text: statusText
            );

            if (allowedToDeleteThisQueueItem)
            {
                if (deleted)
                {
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
        }

        private static InlineKeyboardMarkup GenerateKeyboard(
            int currentId,
            int previousId,
            int nextId,
            bool isEarliestItem,
            bool isLatestItem,
            QueueItem.QueueStatus status,
            TelegramUser user
        )
        {
            var statusText = status.ToString();

            var deleteString = Localization.GetPhrase(Localization.Phrase.Delete, user);
            var nextString = Localization.GetPhrase(Localization.Phrase.Later, user);
            var lastString = Localization.GetPhrase(Localization.Phrase.Latest, user);
            var previousString = Localization.GetPhrase(Localization.Phrase.Sooner, user);
            var firstString = Localization.GetPhrase(Localization.Phrase.Soonest, user);

            if (status == QueueItem.QueueStatus.Posted)
            {
                nextString = "Earlier";
                lastString = "Earliest";
                previousString = Localization.GetPhrase(Localization.Phrase.Later, user);
                firstString = Localization.GetPhrase(Localization.Phrase.Latest, user);
            }

            string earlierButton = isEarliestItem ? $"{lastString} ⏩" : $"⬅️ {previousString}";
            string laterButton = isLatestItem ? $"⏪ {firstString}" : $"{nextString} ➡️";

            bool postedStatus = (status == QueueItem.QueueStatus.Posted) ? true : false;
            bool queuedStatus = (status == QueueItem.QueueStatus.Queued) ? true : false;

            return new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(earlierButton, $"view_nav:{previousId.ToString()}:{statusText}"),
                    InlineKeyboardButton.WithCallbackData(laterButton, $"view_nav:{nextId.ToString()}:{statusText}"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData($"🗑 {deleteString}", $"view_del:{currentId}:{statusText}")
                },
                GenerateStatusKeyboardButton(
                    postedStatus,
                    queuedStatus
                )
            });
        }

        private static InlineKeyboardButton[] GenerateStatusKeyboardButton(
            bool postedStatus = false,
            bool queuedStatus = false
        )
        {
            const string tick = "✔️ ";

            string postedStatusIcon = "📥 ";
            string queuedStatusIcon = "📤 ";

            string action = "nav";

            if (postedStatus || queuedStatus)
            {
                postedStatusIcon = postedStatus ? tick : null;
                queuedStatusIcon = queuedStatus ? tick : null;
            }

            return new[]
            {
                InlineKeyboardButton.WithCallbackData($"{queuedStatusIcon}Queued", $"view_{action}:0:Queued"),
                InlineKeyboardButton.WithCallbackData($"{postedStatusIcon}Posted", $"view_{action}:0:Posted")
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

                var caption = QueueService.GetQueueCaption(selectedQueuedItem, true).CaptionText;

                var keyboard = GenerateKeyboard(
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
                    Media = placeholderImage
                },
                messageId: messageId
            );

            var caption = "";

            switch (status)
            {
                case QueueItem.QueueStatus.Posted:
                    caption = MessageUtilities.CreateWarningMessage("Nothing has been posted");
                    break;
                case QueueItem.QueueStatus.Queued:
                default:
                    caption = MessageUtilities.CreateWarningMessage("Nothing in selected queue");
                    break;
            }

            await Program.BotClient.EditMessageCaptionAsync(
                caption: caption,
                chatId: chatId,
                messageId: messageId,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    GenerateStatusKeyboardButton()
                }),
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

        /*
        public static void Invoke(CommandHandlerArguments arguments)
        {
            long chatId = arguments.TelegramMeta.ChatId;
            bool isCallback = false;

            if (arguments.TelegramCallbackQueryEvent != null)
            {
                isCallback = true;
                Program.BotClient.DeleteMessageAsync(
                    chatId: chatId,
                    messageId: arguments.TelegramMeta.MessageId
                );
            }

            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                GetQueueItemsDataReturn queueItems = null;

                if (isCallback)
                {
                    queueItems = GetQueueItemsData(
                        arguments.TelegramUser,
                        0,
                        arguments.CommandArguments[1].ToString()
                    );
                }
                else
                {
                    queueItems = GetQueueItemsData(
                        arguments.TelegramUser,
                        0
                    );
                }

                if (queueItems.HasItems)
                {
                    var selectedQueuedItem = queueItems.SelectedQueuedItem;
                    var controlKeyboard = queueItems.Keyboard;
                    var caption = queueItems.Caption;

                    switch (selectedQueuedItem.Type)
                    {
                        case QueueItem.MediaType.Image:
                            Program.BotClient.SendPhotoAsync(
                                caption: caption,
                                chatId: chatId,
                                parseMode: ParseMode.Html,
                                photo: selectedQueuedItem.MediaUrl,
                                replyMarkup: controlKeyboard
                            );
                            break;
                        case QueueItem.MediaType.Video:
                            Program.BotClient.SendVideoAsync(
                                caption: caption,
                                chatId: chatId,
                                parseMode: ParseMode.Html,
                                replyMarkup: controlKeyboard,
                                video: selectedQueuedItem.MediaUrl
                            );
                            break;
                    }
                }
                else
                {
                    //throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.NothingIsQueued, arguments.TelegramUser));
                    HandleEmptyQueue(
                        chatId,
                        arguments.TelegramUser
                    );

                    if (isCallback)
                    {
                        Program.BotClient.AnswerCallbackQueryAsync(
                            callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id,
                            text: "Blah"
                        );
                    }
                }
            }
            else
            {
                throw new CommandHandlerException(Localization.GetPhrase(Localization.Phrase.NoPermissionViewQueue, arguments.TelegramUser));
            }
        }

        public static async Task InvokeChange(
            CommandHandlerArguments arguments,
            int queueItemId = 0
        )
        {
            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                var queueItemsData = GetQueueItemsData(
                    arguments.TelegramUser,
                    queueItemId,
                    arguments.CommandArguments[1].ToString());

                if (queueItems.HasItems)
                {
                    var selectedQueuedItem = queueItems.SelectedQueuedItem;
                    var controlKeyboard = queueItems.Keyboard;
                    var caption = queueItems.Caption;

                    switch (selectedQueuedItem.Type)
                    {
                        case QueueItem.MediaType.Image:
                            await Program.BotClient.EditMessageMediaAsync(
                                chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                                media: new InputMediaPhoto
                                {
                                    Media = selectedQueuedItem.MediaUrl
                                },
                                messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                            );
                            break;
                        case QueueItem.MediaType.Video:
                            await Program.BotClient.EditMessageMediaAsync(
                                chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                                media: new InputMediaVideo
                                {
                                    Media = selectedQueuedItem.MediaUrl
                                },
                                messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                            );
                            break;
                    }

                    await Program.BotClient.EditMessageCaptionAsync(
                        caption: caption,
                        chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                        messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId,
                        replyMarkup: controlKeyboard,
                        parseMode: ParseMode.Html
                    );
                }
                else
                {
                    await Program.BotClient.DeleteMessageAsync(
                        chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                        messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                    );

                    //MessageUtilities.SendWarningMessage(Localization.GetPhrase(Localization.Phrase.NothingIsQueued, arguments.TelegramUser), arguments.TelegramCallbackQueryEvent);
                    HandleEmptyQueue(
                        arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                        arguments.TelegramUser
                    );
                }

                await Program.BotClient.AnswerCallbackQueryAsync(
                    callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id
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

        public static async Task InvokeDelete(
            CommandHandlerArguments arguments,
            int queueItemId = 0
        )
        {
            var queueItemsData = GetQueueItemsData(
                arguments.TelegramUser,
                queueItemId,
                arguments.CommandArguments[1].ToString());
            var userId = arguments.TelegramCallbackQueryEvent.CallbackQuery.From.Id;

            bool allowedToDeleteThisQueueItem = false;
            var statusText = MessageUtilities.CreateWarningMessage(Localization.GetPhrase(Localization.Phrase.NoPermissionRemovePost, arguments.TelegramUser));

            if (AuthorData.CanAuthorQueue(userId))
            {
                if (queueItems.SelectedQueuedItem.Author.TelegramId == userId)
                {
                    allowedToDeleteThisQueueItem = true;
                }
                else
                {
                    if (AuthorData.CanAuthorDeleteOthers(userId))
                    {
                        allowedToDeleteThisQueueItem = true;
                    }
                }
            }

            if (allowedToDeleteThisQueueItem)
            {
                QueueService.RemoveQueueItem(queueItems.SelectedQueuedItem.Id);
                statusText = MessageUtilities.CreateSuccessMessage(
                    Localization.GetPhrase(
                        Localization.Phrase.DeletedFromQueue, arguments.TelegramUser,
                        new string[] {
                            queueItems.SelectedQueuedItem.Id.ToString()
                        }
                    )
                );

                try
                {
                    if (queueItems.SelectedQueuedItem.MessageId != 0)
                    {
                        await Program.BotClient.DeleteMessageAsync(
                            chatId: Models.AppSettings.Config_ChannelId,
                            messageId: Convert.ToInt32(queueItems.SelectedQueuedItem.MessageId)
                        );
                        statusText = MessageUtilities.CreateSuccessMessage($"Deleted {queueItems.SelectedQueuedItem.MessageId} from channel");
                    }
                }
                catch (Exception)
                {
                    statusText = MessageUtilities.CreateWarningMessage($"Unable to delete {queueItems.SelectedQueuedItem.MessageId} from channel");
                }
            }

            await Program.BotClient.AnswerCallbackQueryAsync(
                callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id,
                text: statusText
            );

            if (allowedToDeleteThisQueueItem)
            {
                int idToNavigateTo = queueItems.PreviousQueuedItem.Id;

                if (queueItems.IsEarliestItem)
                {
                    idToNavigateTo = queueItems.NextQueuedItem.Id;
                }

                await InvokeChange(arguments, idToNavigateTo);
            }
        }

        private static void HandleEmptyQueue(
            long chatId,
            TelegramUser user
        )
        {
            Program.BotClient.SendTextMessageAsync(
                chatId: chatId,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    GenerateStatusKeyboardButton()
                }),
                text: MessageUtilities.CreateWarningMessage(Localization.GetPhrase(Localization.Phrase.NothingIsQueued, user))
            );
        }
        */
    }
}