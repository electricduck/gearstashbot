using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using StashBot.Data;
using StashBot.Exceptions;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Models.ReturnModels.ViewCommandHandlerReturns;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class ViewCommandHandler
    {
        public static void Invoke(CommandHandlerArguments arguments)
        {
            bool showAllItems = false;

            /*if(arguments.CommandArguments != null)
            {
                if(arguments.CommandArguments[0].ToString() == "all")
                {
                    showAllItems = true;
                }
                else
                {
                    throw new ArgumentException();
                }
            }*/

            if (AuthorData.CanAuthorQueue(TelegramUtilities.GetUserId(arguments.TelegramMessageEvent)))
            {
                var queueItems = GetQueueItems(0, showAllItems);

                if (queueItems.HasItems)
                {
                    var selectedQueuedItem = queueItems.SelectedQueuedItem;
                    var controlKeyboard = queueItems.Keyboard;
                    var caption = queueItems.Caption;

                    switch (selectedQueuedItem.Type)
                    {
                        case QueueItem.MediaType.Image:
                            TelegramApiService.SendPhoto(
                                new SendPhotoArguments
                                {
                                    Caption = caption,
                                    Photo = selectedQueuedItem.MediaUrl,
                                    ReplyMarkup = controlKeyboard
                                },
                                Program.BotClient,
                                arguments.TelegramMessageEvent
                            );
                            break;
                        case QueueItem.MediaType.Video:
                            TelegramApiService.SendVideo(
                                new SendVideoArguments
                                {
                                    Caption = caption,
                                    ReplyMarkup = controlKeyboard,
                                    Video = selectedQueuedItem.MediaUrl
                                },
                                Program.BotClient,
                                arguments.TelegramMessageEvent
                            );
                            break;
                    }
                }
                else
                {
                    throw new CommandHandlerException("Nothing is queued");
                }
            }
            else
            {
                throw new CommandHandlerException("You do not have permission to view the queue");
            }
        }

        public static async Task InvokeChange(
            CommandHandlerArguments arguments,
            int queueItemId = 0
        )
        {
            if (AuthorData.CanAuthorQueue(arguments.TelegramUser.Id))
            {
                var queueItems = GetQueueItems(queueItemId);

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

                    MessageUtilities.SendWarningMessage("Nothing is queued", arguments.TelegramCallbackQueryEvent);
                }
            }
            else
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                );

                throw new CommandHandlerException("You do not have permission to view the queue");
            }
        }

        public static async Task InvokeDelete(
            CommandHandlerArguments arguments,
            int queueItemId = 0
        )
        {
            var queueItems = GetQueueItems(queueItemId);
            var userId = arguments.TelegramCallbackQueryEvent.CallbackQuery.From.Id;

            var selectedQueuedItem = queueItems.SelectedQueuedItem;
            var previousQueuedItem = queueItems.PreviousQueuedItem;
            var nextQueuedItem = queueItems.NextQueuedItem;
            var controlKeyboard = queueItems.Keyboard;

            bool canDeleteThisQueueItem = false;
            var statusText = MessageUtilities.CreateWarningMessage($"You do not have permission to remove this post");

            if (AuthorData.CanAuthorQueue(userId))
            {
                if (selectedQueuedItem.Author.TelegramId == userId)
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
                QueueService.RemoveQueueItem(selectedQueuedItem.Id);
                statusText = MessageUtilities.CreateSuccessMessage($"Deleted #{selectedQueuedItem.Id} from queue");
            }

            await Program.BotClient.AnswerCallbackQueryAsync(
                callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id,
                text: statusText
            );

            if (canDeleteThisQueueItem)
            {
                int idToNavigateTo = previousQueuedItem.Id;

                if (queueItems.IsEarliestItem)
                {
                    idToNavigateTo = nextQueuedItem.Id;
                }

                await InvokeChange(arguments, idToNavigateTo);
            }
        }

        private static GetQueueItemsReturn GetQueueItems(int id = 0, bool showAllItems = false)
        {
            GetQueueItemsReturn returnModel = new GetQueueItemsReturn { };

            List<QueueItem> queue = showAllItems ? QueueData.ListQueueItems() : QueueData.ListQueuedQueueItems();

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
                    isLatestItem
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
            bool isLatestItem
        )
        {
            string earlierButton = isEarliestItem ? "Latest ⏩" : "⬅️ Sooner";
            string laterButton = isLatestItem ? "⏪ Soonest" : "Later ➡️";

            return new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(earlierButton, $"view_prev:{previousId.ToString()}"),
                    InlineKeyboardButton.WithCallbackData(laterButton, $"view_next:{nextId.ToString()}"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("🗑 Delete", $"view_del:{currentId}"),
                    /*InlineKeyboardButton.WithCallbackData("📩 Post", $"view_postNow:{currentId}"),
                    InlineKeyboardButton.WithCallbackData("⬆️ Bump", $"view_bump:{currentId}")*/
                }
            });
        }
    }
}