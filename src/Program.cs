﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using GearstashBot.Handlers;
using GearstashBot.Models;
using GearstashBot.Services;
using GearstashBot.Utilities;

namespace GearstashBot
{
    class Program
    {
        public static ITelegramBotClient BotClient;

        private static string DefaultConfig = @"{
    ""apiKeys"": {
        ""telegram"": ""1234567890:AbC_dEfGhIjKlMnOpQrStUvWxYz""
    },
    ""config"": {
        ""backupDatabase"": true,
        ""channel"": -1000000000000,
        ""name"": ""Gearstash Bot"",
        ""owner"": ""OopsIForgotToSetTheOwner"",
        ""poll"": true,
        ""postInterval"": 30000,
        ""warnOnDuplicate"": true
    }
}";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                MessageUtilities.PrintStartupMessage();

                try
                {
                    MessageUtilities.PrintInfoMessage("Updating required pip packages...");
                    UpdatePipPackage("gallery-dl");
                    UpdatePipPackage("youtube-dl");
                }
                catch (Win32Exception e)
                {
                    if (e.NativeErrorCode == 2)
                    {
                        throw new Exception("'python3' is not installed.");
                    }
                }

                MessageUtilities.PrintInfoMessage("Setting up app...");
                SetupApp();

                MessageUtilities.PrintInfoMessage("Migrating database...");
                DbUtilities.MigrateDatabase();

                if (AppSettings.Config_BackupDatabase)
                {
                    MessageUtilities.PrintInfoMessage("Backing up database...");
                    DbUtilities.BackupDatabase();
                }

                BotClient = new TelegramBotClient(AppSettings.ApiKeys_Telegram);

                MessageUtilities.PrintInfoMessage("Connecting to Telegram...");
                if (!BotClient.TestApiAsync().Result)
                {
                    throw new Exception("Telegram API key invalid.");
                }
            }
            catch (Exception e)
            {
                MessageUtilities.PrintErrorMessage(e, Guid.Empty);
                Environment.Exit(1);
            }

            MessageUtilities.PrintSuccessMessage("Initial startup successful");

            try
            {
                MessageUtilities.PrintInfoMessage("Listening to messages...");
                BotClient.OnMessage += BotEventHandler.Bot_OnMessage;
                BotClient.OnCallbackQuery += BotEventHandler.Bot_OnCallbackQuery;
                BotClient.OnInlineQuery += BotEventHandler.Bot_OnInlineQueryRecieved;
                BotClient.StartReceiving();

                if (AppSettings.Config_Poll)
                {
                    MessageUtilities.PrintInfoMessage("Polling queue...");
                    QueueService.PollQueue();
                }
                else
                {
                    MessageUtilities.PrintWarningMessage("Polling has been disabled in the configuration");
                }
            }
            catch (Exception e)
            {
                MessageUtilities.PrintErrorMessage(e, Guid.Empty);
            }

            Thread.Sleep(int.MaxValue);
        }

        private static void SetupApp()
        {
            if (!Directory.Exists($"config/"))
            {
                Directory.CreateDirectory($"config/");
            }

            if (!File.Exists($"config/appsettings.json"))
            {
                File.WriteAllText($"config/appsettings.json", DefaultConfig);
                throw new Exception($"Settings file did not exist. Please edit 'config/appsettings.json' and re-run.");
            }

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"config/appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            // TODO: Handle nulls
            AppSettings.ApiKeys_Telegram = configuration.GetSection("apiKeys")["telegram"];
            AppSettings.Config_BackupDatabase = Convert.ToBoolean(configuration.GetSection("config")["backupDatabase"]);
            AppSettings.Config_ChannelId = Convert.ToInt64(configuration.GetSection("config")["channel"]);
            AppSettings.Config_Name = (configuration.GetSection("config").GetChildren().Any(i => i.Key == "name")) ? configuration.GetSection("config")["name"] : "Gearstash Bot";
            AppSettings.Config_Owner = "@" + configuration.GetSection("config")["owner"].Replace("@", "");
            AppSettings.Config_Poll = Convert.ToBoolean(configuration.GetSection("config")["poll"]);
            AppSettings.Config_PostInterval = Convert.ToInt32(configuration.GetSection("config")["postInterval"]);
            AppSettings.Config_WarnOnDuplicate = Convert.ToBoolean(configuration.GetSection("config")["warnOnDuplicate"]);
        }

        private static string UpdatePipPackage(string package)
        {
            // TODO: Handle pip not installed
            //       Handle pip outputs
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python3",
                    Arguments = $"-m pip install --upgrade {package}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}
