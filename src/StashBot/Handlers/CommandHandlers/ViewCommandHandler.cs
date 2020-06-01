using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using StashBot.Data;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Models.ReturnModels.ReturnStatusEnums;
using StashBot.Models.ReturnModels.ViewCommandHandlerReturns;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class ViewCommandHandler
    {
        public static ViewInvokeReturn Invoke(
            MessageEventArgs telegramMessageEvent,
            int queueItemId = 0
        )
        {
            ViewInvokeReturn returnModel = new ViewInvokeReturn { };

            if (AuthorData.CanAuthorQueue(TelegramUtilities.GetUserId(telegramMessageEvent)))
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
                            returnModel.SendPhotoArguments = new SendPhotoArguments
                            {
                                Caption = caption,
                                Photo = selectedQueuedItem.MediaUrl,
                                ReplyMarkup = controlKeyboard
                            };
                            break;
                        case QueueItem.MediaType.Video:
                            returnModel.SendVideoArguments = new SendVideoArguments
                            {
                                Caption = caption,
                                ReplyMarkup = controlKeyboard,
                                Video = selectedQueuedItem.MediaUrl
                            };
                            break;
                    }

                    returnModel.Status = ViewInvokeReturnStatus.FoundQueuedPosts;
                }
                else
                {
                    returnModel.Status = ViewInvokeReturnStatus.NothingQueued;
                }
            }
            else
            {
                returnModel.HasPermission = false;
            }

            return returnModel;
        }

        public static async Task InvokeChange(
            CallbackQueryEventArgs callbackQueryEventArgs,
            int queueItemId = 0
        )
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
                            chatId: callbackQueryEventArgs.CallbackQuery.Message.Chat.Id,
                            media: new InputMediaPhoto
                            {
                                Media = selectedQueuedItem.MediaUrl
                            },
                            messageId: callbackQueryEventArgs.CallbackQuery.Message.MessageId
                        );
                        break;
                    case QueueItem.MediaType.Video:
                        await Program.BotClient.EditMessageMediaAsync(
                            chatId: callbackQueryEventArgs.CallbackQuery.Message.Chat.Id,
                            media: new InputMediaVideo
                            {
                                Media = selectedQueuedItem.MediaUrl
                            },
                            messageId: callbackQueryEventArgs.CallbackQuery.Message.MessageId
                        );
                        break;
                }

                await Program.BotClient.EditMessageCaptionAsync(
                    caption: caption,
                    chatId: callbackQueryEventArgs.CallbackQuery.Message.Chat.Id,
                    messageId: callbackQueryEventArgs.CallbackQuery.Message.MessageId,
                    replyMarkup: controlKeyboard,
                    parseMode: ParseMode.Html
                );
            }
            else
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: callbackQueryEventArgs.CallbackQuery.Message.Chat.Id,
                    messageId: callbackQueryEventArgs.CallbackQuery.Message.MessageId
                );

                MessageUtilities.SendWarningMessage("Nothing is queued", callbackQueryEventArgs);
            }
        }

        public static async Task InvokeDelete(
            CallbackQueryEventArgs callbackQueryEventArgs,
            int queueItemId = 0
        )
        {
            var queueItems = GetQueueItems(queueItemId);
            var userId = callbackQueryEventArgs.CallbackQuery.From.Id;

            var selectedQueuedItem = queueItems.SelectedQueuedItem;
            var previousQueuedItem = queueItems.PreviousQueuedItem;
            var controlKeyboard = queueItems.Keyboard;

            bool canDeleteThisQueueItem = false;
            var statusText = MessageUtilities.CreateWarningMessage($"You do not have permission to remove this post"); ;

            if (AuthorData.CanAuthorQueue(userId))
            {

                if (selectedQueuedItem.AuthorTelegramId == userId)
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
                QueueService.RemoveItem(selectedQueuedItem.Id);
                statusText = MessageUtilities.CreateSuccessMessage($"Deleted #{selectedQueuedItem.Id} from queue");
            }

            await Program.BotClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQueryEventArgs.CallbackQuery.Id,
                text: statusText
            );

            if (canDeleteThisQueueItem)
            {
                await InvokeChange(callbackQueryEventArgs, previousQueuedItem.Id);
            }
        }

        private static GetQueueItemsReturn GetQueueItems(int id)
        {
            GetQueueItemsReturn returnModel = new GetQueueItemsReturn { };

            List<QueueItem> queue = QueueData.ListQueuedQueueItems();

            if (queue.Count() > 0)
            {
                QueueItem selectedQueuedItem = (id == 0) ? queue[0] : queue.Where(q => q.Id == id).FirstOrDefault();
                int minIndex = 0;
                int maxIndex = queue.Count() - 1;

                int currentIndex = queue.IndexOf(selectedQueuedItem);
                int previousIndex = currentIndex - 1;
                int nextIndex = currentIndex + 1;

                QueueItem previousQueuedItem = (previousIndex < minIndex) ? queue[maxIndex] : queue[previousIndex];
                QueueItem nextQueuedItem = (nextIndex > maxIndex) ? queue[minIndex] : queue[nextIndex];

                var caption = QueueService.GetQueueCaption(selectedQueuedItem, true).CaptionText;

                var keyboard = GenerateKeyboard(
                    selectedQueuedItem.Id,
                    previousQueuedItem.Id,
                    nextQueuedItem.Id
                );

                returnModel.SelectedQueuedItem = selectedQueuedItem;
                returnModel.PreviousQueuedItem = previousQueuedItem;
                returnModel.NextQueuedItem = nextQueuedItem;
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
            int nextId
        )
        {
            return new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Earlier", $"view_prev:{previousId.ToString()}"),
                    InlineKeyboardButton.WithCallbackData("Later ➡️", $"view_next:{nextId.ToString()}"),
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