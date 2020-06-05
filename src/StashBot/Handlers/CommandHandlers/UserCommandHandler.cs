using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using StashBot.Data;
using StashBot.Exceptions;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class UserCommandHandler
    {
        public static Help Help = new Help
        {
            Arguments = new List<HelpArgument> {
              new HelpArgument {
                  Example = "63391517, @theducky",
                  Explanation = "User ID of Telegram user, or username of user if it exists on the database",
                  Name = "User ID",
                  Position = 1
              },
            },
            Command = "user",
            Description = "Manage user permissions"
        };

        public static void Invoke(CommandHandlerArguments arguments, bool reload = false)
        {
            bool hasPermission = AuthorData.CanAuthorManageAuthors(arguments.TelegramUser.Id);

            if (arguments.CommandArguments == null)
            {
                if (hasPermission)
                {
                    arguments.CommandArguments = new string[] { arguments.TelegramUser.Id.ToString() };
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            if (hasPermission)
            {
                long authorId = 0;
                string authorIdOrUsernameArgument = arguments.CommandArguments[0];

                if (!(long.TryParse(authorIdOrUsernameArgument, out authorId)))
                {
                    if (arguments.CommandArguments[0].ToString().StartsWith("@"))
                    {
                        Author authorByUsername = AuthorData.GetAuthorByTelegramUsername(arguments.CommandArguments[0], false);

                        if (authorByUsername != null)
                        {
                            authorId = authorByUsername.TelegramId;
                        }
                        else
                        {
                            throw new CommandHandlerException($"Cannot find <code>{arguments.CommandArguments[0]}</code>");
                        }
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }

                Author author = AuthorData.GetAuthor(authorId, true);

                if (author.TelegramId == arguments.TelegramUser.Id)
                {
                    if (!(
                        author.TelegramName == arguments.TelegramUser.Name &&
                        author.TelegramUsername == arguments.TelegramUser.Username
                    ))
                    {
                        author = AuthorData.UpdateAuthorTelegramProfile(arguments.TelegramUser);
                    }
                }

                string authorNameText = (String.IsNullOrEmpty(author.TelegramName)) ? "<i>(Not set)</i>" : author.TelegramName;
                string authorUsernameText = (String.IsNullOrEmpty(author.TelegramUsername)) ? "<i>(Not set)</i>" : author.TelegramUsername;
                string authorLastUpdatedText = author.TelegramDetailsLastUpdatedAt.ToString("dd-MMM-yy hh:mm:ss zz");
                int authorPostCount = AuthorData.CountAuthorQueue(author.TelegramId);
                int queueCount = QueueData.CountQueueItems();
                decimal queuePercentage = 0;

                if(queueCount > 0)
                {
                    queuePercentage = ((decimal)authorPostCount / queueCount) * 100;
                }

                var userPermissionKeyboard = GetPermissionKeyboard(author);
                var userDetailsText = $@"👤 <b>User:</b> <code>{authorId}</code>
—
<b>Name:</b> {authorNameText}
<b>Username:</b> {authorUsernameText}
<b>Profile Updated:</b> {authorLastUpdatedText}
<b>Posts:</b> {authorPostCount} ({queuePercentage.ToString("0.00")}%)";

                TelegramApiService.SendTextMessage(
                    new SendTextMessageArguments
                    {
                        ReplyMarkup = userPermissionKeyboard,
                        Text = userDetailsText
                    },
                    Program.BotClient,
                    arguments.TelegramMessageEvent
                );
            }
            else
            {
                throw new CommandHandlerException("You do not have permission to manage users");
            }
        }

        public async static Task InvokeSetPermission(CommandHandlerArguments arguments)
        {
            if (AuthorData.CanAuthorManageAuthors(arguments.TelegramUser.Id))
            {
                Author author = AuthorData.GetAuthor(Convert.ToInt64(arguments.CommandArguments[0]), false);
                string permission = arguments.CommandArguments[1].ToString();
                bool setting = Convert.ToBoolean(arguments.CommandArguments[2].ToString());
                bool settingUpdated = true;

                switch (permission)
                {
                    case "CanDeleteOthers":
                        AuthorData.SetAuthorDeleteOthersPermission(author.TelegramId, setting);
                        author.CanDeleteOthers = setting;
                        break;
                    case "CanFlushQueue":
                        AuthorData.SetAuthorFlushQueuePermission(author.TelegramId, setting);
                        author.CanFlushQueue = setting;
                        break;
                    case "CanManageAuthors":
                        if (author.CanManageAuthors == true && author.TelegramId == arguments.TelegramUser.Id)
                        {
                            settingUpdated = false;
                        }
                        else
                        {
                            AuthorData.SetAuthorManageAuthorsPermission(author.TelegramId, setting);
                            author.CanManageAuthors = setting;
                        }
                        break;
                    case "CanQueue":
                        AuthorData.SetAuthorQueuePermission(author.TelegramId, setting);
                        author.CanQueue = setting;
                        break;
                }

                if (settingUpdated)
                {
                    await Program.BotClient.EditMessageReplyMarkupAsync(
                        arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                        arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId,
                        GetPermissionKeyboard(author)
                    );
                }
                else
                {
                    await Program.BotClient.AnswerCallbackQueryAsync(
                        callbackQueryId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Id,
                        text: MessageUtilities.CreateWarningMessage($"You cannot remove this permission from yourself")
                    );
                }
            }
            else
            {
                await Program.BotClient.DeleteMessageAsync(
                    chatId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.Chat.Id,
                    messageId: arguments.TelegramCallbackQueryEvent.CallbackQuery.Message.MessageId
                );

                throw new CommandHandlerException("You do not have permission to manage users");
            }
        }

        public static void InvokeSetup(CommandHandlerArguments arguments)
        {
            int authorCount = AuthorData.CountAuthors();
            if (authorCount == 0)
            {
                AuthorData.CreateAuthor(arguments.TelegramUser);
                AuthorData.SetAuthorManageAuthorsPermission(arguments.TelegramUser.Id, true);
                MessageUtilities.SendSuccessMessage($"Welcome to StashBot! Set your permissions with <code>/user {arguments.TelegramUser.Id}</code>", arguments.TelegramMessageEvent);
            }
        }

        private static InlineKeyboardMarkup GetPermissionKeyboard(Author author)
        {
            const string tick = "✔️";
            const string cross = "✖️";

            string canDeleteOthersStatus = author.CanDeleteOthers ? tick : cross;
            string canFlushQueueStatus = author.CanFlushQueue ? tick : cross;
            string canManageAuthorsStatus = author.CanManageAuthors ? tick : cross;
            string canQueueStatus = author.CanQueue ? tick : cross;

            return new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData($"{canQueueStatus} Queue", $"user_perm:{author.TelegramId}:CanQueue:{!author.CanQueue}")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData($"{canDeleteOthersStatus} Delete Others", $"user_perm:{author.TelegramId}:CanDeleteOthers:{!author.CanDeleteOthers}")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData($"{canFlushQueueStatus} Flush Queue", $"user_perm:{author.TelegramId}:CanFlushQueue:{!author.CanFlushQueue}")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData($"{canManageAuthorsStatus} Manage Users", $"user_perm:{author.TelegramId}:CanManageAuthors:{!author.CanManageAuthors}")
                }
            });
        }
    }
}