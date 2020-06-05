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
        public static Help Help = new Help
        {
            Command = "view",
            Description = "View and manage posts in queue"
        };

        public static void Invoke(CommandHandlerArguments arguments)
        {
            long chatId = 0;

            if (arguments.TelegramMessageEvent != null)
            {
                chatId = arguments.TelegramMessageEvent.Message.Chat.Id;
            }
            else if (arguments.TelegramCallbackQueryEvent != null)
            {
                chatId = arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id;

                Program.BotClient.DeleteMessageAsync(
                    chatId: chatId,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                );
            }

            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                GetQueueItemsReturn queueItems = null;

                if (arguments.TelegramCallbackQueryEvent != null)
                {
                    queueItems = GetQueueItems(
                        arguments.TelegramUser,
                        0,
                        arguments.CommandArguments[1].ToString()
                    );
                }
                else
                {
                    queueItems = GetQueueItems(
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
                var queueItems = GetQueueItems(
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
            var queueItems = GetQueueItems(
                arguments.TelegramUser,
                queueItemId,
                arguments.CommandArguments[1].ToString());
            var userId = arguments.TelegramCallbackQueryEvent.CallbackQuery.From.Id;

            bool canDeleteThisQueueItem = false;
            var statusText = MessageUtilities.CreateWarningMessage(Localization.GetPhrase(Localization.Phrase.NoPermissionRemovePost, arguments.TelegramUser));

            if (AuthorData.CanAuthorQueue(userId))
            {
                if (queueItems.SelectedQueuedItem.Author.TelegramId == userId)
                {
                    canDeleteThisQueueItem = true;
                }
                else
                {
                    if (AuthorData.CanAuthorDeleteOthers(userId))
                    {
                        canDeleteThisQueueItem = true;
                    }
                }
            }

            if (canDeleteThisQueueItem)
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

            if (canDeleteThisQueueItem)
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

        private static GetQueueItemsReturn GetQueueItems(TelegramUser user, int id = 0, string statusText = "Queued")
        {
            GetQueueItemsReturn returnModel = new GetQueueItemsReturn { };

            List<QueueItem> queue = null;
            QueueItem.QueueStatus status = QueueItem.QueueStatus.Queued;
            Enum.TryParse(statusText, out status);

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
                    InlineKeyboardButton.WithCallbackData(earlierButton, $"view_prev:{previousId.ToString()}:{statusText}"),
                    InlineKeyboardButton.WithCallbackData(laterButton, $"view_next:{nextId.ToString()}:{statusText}"),
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
            bool queuedStatus = false,
            bool tryAgain = true
        )
        {
            const string tick = "✔️ ";

            string postedStatusIcon = "📥 ";
            string queuedStatusIcon = "📤 ";

            string action = "prev";

            if (postedStatus || queuedStatus)
            {
                postedStatusIcon = postedStatus ? tick : null;
                queuedStatusIcon = queuedStatus ? tick : null;
            }

            if (tryAgain)
            {
                action = "view";
            }

            return new[]
            {
                InlineKeyboardButton.WithCallbackData($"{queuedStatusIcon}Queued", $"view_{action}:0:Queued"),
                InlineKeyboardButton.WithCallbackData($"{postedStatusIcon}Posted", $"view_{action}:0:Posted")
            };
        }
    }
}