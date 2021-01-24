using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using GearstashBot.Exceptions;
using GearstashBot.Handlers.CommandHandlers;
using GearstashBot.I18n;
using GearstashBot.Models.ArgumentModels;
using GearstashBot.Utilities;
using GearstashBot.Data;

namespace GearstashBot.Handlers
{
    public class BotEventHandler
    {
        private static Regex CallbackRegex = new Regex(@"^([a-z_]{1,100})([:]){0,1}([\/a-zA-Z0-9_:.,-@ ]*)$");
        private static Regex CommandRegex = new Regex(@"^([\/][a-z]{1,100})([ ])*([\/a-zA-Z0-9_:.,*-@ ]*)$");
        private static Regex UrlRegex = new Regex(@"(http|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?");

        public static void Bot_OnMessage(object sender, MessageEventArgs telegramMessageEvent)
        {
            Task.Run(() =>
            {
                CommandHandlerArguments arguments = new CommandHandlerArguments { };
                string messageText = telegramMessageEvent.Message.Text != null ? telegramMessageEvent.Message.Text.ToString() : "";
                Match matchedUrl = UrlRegex.Match(messageText);

                if(matchedUrl.Success && !messageText.StartsWith("/"))
                {
                    messageText = $"/post {matchedUrl.Groups[0].Value}";
                }

                try
                {
                    if (!String.IsNullOrEmpty(messageText))
                    {
                        Match matchedCommand = CommandRegex.Match(messageText);
                        
                        arguments.TelegramUser = TelegramUtilities.GetTelegramUser(telegramMessageEvent);
                        AuthorData.UpdateAuthorTelegramProfile(arguments.TelegramUser);
                        AuthorData.UpdateAuthorLastAccess(arguments.TelegramUser);

                        if (messageText.StartsWith("/"))
                        {
                            arguments.Command = matchedCommand.Groups[1].Value.Replace("/", "").ToLower();
                            arguments.CommandArgument = matchedCommand.Groups[3].Value;
                            arguments.CommandArguments = matchedCommand.Groups[3].Value.Split(" ");
                            arguments.TelegramMessageEvent = telegramMessageEvent;

                            if (String.IsNullOrEmpty(arguments.CommandArguments[0].ToString()))
                            {
                                arguments.CommandArguments = null;
                            }

                            switch (arguments.Command)
                            {
                                case "catpls":
                                    CatPlsCommandHandler.Invoke(arguments);
                                    break;
                                case "info":
                                    InfoCommandHandler.Invoke(arguments);
                                    break;
                                case "post":
                                    PostCommandHandler.Invoke(arguments);
                                    break;
                                case "start":
                                    UserCommandHandler.InvokeSetup(arguments);
                                    break;
                                case "user":
                                case "set":
                                    UserCommandHandler.Invoke(arguments);
                                    break;
                                case "view":
                                    ViewCommandHandler.Invoke(arguments);
                                    break;
                            }
                        }
                    }
                }
                catch (ArgumentException)
                {
                    MessageUtilities.SendWarningMessage(
                        Localization.GetPhrase(
                            Localization.Phrase.InvalidArgs,
                            arguments.TelegramUser,
                            new string[] {
                                arguments.Command
                            }
                        ),
                        telegramMessageEvent);
                }
                catch (CommandHandlerException e)
                {
                    MessageUtilities.SendWarningMessage(e.Message, telegramMessageEvent);
                }
                catch (Exception e)
                {
                    MessageUtilities.SendErrorMessage(e, telegramMessageEvent);
                }
            });
        }

        public static void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs telegramCallbackQueryEvent)
        {
            Task.Run(async () =>
            {
                CommandHandlerArguments arguments = new CommandHandlerArguments { };

                try
                {
                    Match matchedCommand = CallbackRegex.Match(telegramCallbackQueryEvent.CallbackQuery.Data);

                    arguments.Command = matchedCommand.Groups[1].Value;
                    arguments.CommandArguments = matchedCommand.Groups[3].Value.Split(":");
                    arguments.TelegramCallbackQueryEvent = telegramCallbackQueryEvent;
                    arguments.TelegramUser = TelegramUtilities.GetTelegramUser(telegramCallbackQueryEvent);

                    AuthorData.UpdateAuthorLastAccess(arguments.TelegramUser);

                    switch (arguments.Command)
                    {
                        case "user_perm":
                            await UserCommandHandler.InvokeSetPermission(arguments);
                            break;
                        case "view_del":
                            await ViewCommandHandler.InvokeDelete(arguments);
                            break;
                        case "view_list":
                            await ViewCommandHandler.InvokeList(arguments);
                            break;
                        case "view_nav":
                            await ViewCommandHandler.InvokeChange(arguments);
                            break;
                        case "view_rand":
                            await ViewCommandHandler.InvokeShuffle(arguments);
                            break;
                    }
                }
                catch (CommandHandlerException e)
                {
                    MessageUtilities.SendWarningMessage(e.Message, arguments.TelegramCallbackQueryEvent);
                }
                catch (Exception e)
                {
                    MessageUtilities.SendErrorMessage(e, arguments.TelegramCallbackQueryEvent);
                }
            });
        }

        public static void Bot_OnInlineQueryRecieved(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine("Hello!");
        }
    }
}