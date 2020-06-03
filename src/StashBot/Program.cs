using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using StashBot.Handlers;
using StashBot.Models;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot
{
    class Program
    {
        public static ITelegramBotClient BotClient;

        static void Main(string[] args)
        {
            MessageUtilities.PrintStartupMessage();

            try
            {
                SetupApp();
                MessageUtilities.PrintSuccessMessage("Setup successful");
            }
            catch (Exception e)
            {
                MessageUtilities.PrintErrorMessage(e, Guid.Empty);
            }

            BotClient = new TelegramBotClient(AppSettings.ApiKeys_Telegram);

            bool isApiKeyValid = BotClient.TestApiAsync().Result;

            if (isApiKeyValid)
            {
                MessageUtilities.PrintSuccessMessage("Telegram API key valid");
            }
            else
            {
                MessageUtilities.PrintWarningMessage("Telegram API key invalid");
                Environment.Exit(1);
            }

            try
            {
                MessageUtilities.PrintInfoMessage("Listening to messages");
                BotClient.OnMessage += BotEventHandler.Bot_OnMessage;
                BotClient.OnCallbackQuery += BotEventHandler.Bot_OnCallbackQuery;
                BotClient.OnInlineQuery += BotEventHandler.Bot_OnInlineQueryRecieved;
                BotClient.StartReceiving();
            }
            catch (Exception e)
            {
                MessageUtilities.PrintErrorMessage(e, Guid.Empty);
            }

            try
            {
                if (AppSettings.Config_Poll)
                {
                    MessageUtilities.PrintInfoMessage("Polling queue");
                    QueueService.PollQueue();
                }
                else
                {
                    MessageUtilities.PrintInfoMessage("Polling has been disabled in the configuration");
                }
            }
            catch (Exception e)
            {
                MessageUtilities.PrintErrorMessage(e, Guid.Empty);
            }

            /*SendTextMessageArguments output = new SendTextMessageArguments {
                //ChatId = -1001175466865,
                ChatId = -1001250488587,
                Text = "Test post. Please ignore."
            };
            TelegramApiService.SendTextMessage(output, BotClient, null);*/

            Thread.Sleep(int.MaxValue);
        }
        public static void SetupApp()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            AppSettings.ApiKeys_Telegram = configuration.GetSection("apiKeys")["telegram"];
            AppSettings.Config_ChannelId = Convert.ToInt64(configuration.GetSection("config")["channel"]);
            AppSettings.Config_Owner = configuration.GetSection("config")["owner"];
            AppSettings.Config_Poll = Convert.ToBoolean(configuration.GetSection("config")["poll"]);
            AppSettings.Config_PostInterval = Convert.ToInt32(configuration.GetSection("config")["postInterval"]);
        }
    }
}
