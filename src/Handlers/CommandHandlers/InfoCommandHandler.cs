using System;
using Microsoft.DotNet.PlatformAbstractions;
using GearstashBot.Data;
using GearstashBot.Models;
using GearstashBot.Models.ArgumentModels;
using GearstashBot.Services;
using GearstashBot.Utilities;

namespace GearstashBot.Handlers.CommandHandlers
{
    public class InfoCommandHandler
    {
        public static void Invoke(CommandHandlerArguments arguments)
        {
            var thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            var runtime = ReflectionUtilities.GetEnvironment();
            var version = ReflectionUtilities.GetVersion();
            var versionString = ReflectionUtilities.GetVersionString();

            string processMemoryUsage = Convert.ToDecimal(thisProcess.WorkingSet64 / 1000000).ToString();
            DateTime processStartTime = thisProcess.StartTime;
            string systemHostname = System.Net.Dns.GetHostName();
            string systemOpSys = RuntimeEnvironment.OperatingSystem
                .Replace("alpine", "Alpine").Replace("debian", "Debian").Replace("elementary", "elementaryOS")
                .Replace("ubuntu", "Ubuntu");
            string systemOpSysVersion = RuntimeEnvironment.OperatingSystemVersion;
            string systemTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
            string userId = arguments.TelegramUser.Id.ToString();
            string userLanguageCode = arguments.TelegramUser.Language.ToString();

            TimeSpan timeSinceStart = DateTime.Now.ToUniversalTime().Subtract(processStartTime.ToUniversalTime());
            string uptime = timeSinceStart.ToString("d'd 'h'h 'm'm 's's'");

            int queueAmountInt = QueueData.CountQueuedQueueItems();
            string queueAmount = queueAmountInt.ToString();
            string queueApproxDays = QueueUtlities.CalculateQueueApproxDays(queueAmountInt).ToString("0.00");
            string totalQueueAmount = QueueData.CountQueueItems().ToString();
            string usersAmount = AuthorData.CountAuthors().ToString();

            string name = AppSettings.Config_Name;

            string outputText = $@"<b>{name}</b> | <a href=""https://github.com/electricduck/gearstashbot/commit/{version.Commit}"">{versionString}</a>
—
<i>There is <b>{queueAmount} queued posts</b>, amounting to approximately <b>{queueApproxDays} days</b>; with a total of <b>{totalQueueAmount} posts</b>, <b>{usersAmount} users</b>, and <b>{Constants.Cats} cats</b>.</i> 
—
<b>🤖 Bot</b>
Memory: <code>{processMemoryUsage}mb</code>
️Uptime: <code>{uptime}</code>
Env.: <code>{runtime}</code>
—
<b>⚙️ System</b>
Host: <code>{systemHostname}</code>
OS: <code>{systemOpSys} {systemOpSysVersion}</code>
Time: <code>{systemTime}</code>
—
<b>👤 You</b>
ID: <code>{userId}</code>
Language: <code>{userLanguageCode}</code>";

            TelegramApiService.SendTextMessage(
                new SendTextMessageArguments
                {
                    Text = outputText
                },
                Program.BotClient,
                arguments.TelegramMessageEvent
            );
        }
    }
}
