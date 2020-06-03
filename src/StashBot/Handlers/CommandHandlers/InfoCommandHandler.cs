using System;
using System.Reflection;
using System.Runtime.Versioning;
using Telegram.Bot.Args;
using StashBot.Data;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Models.ReturnModels;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class InfoCommandHandler
    {
        public static ReturnModelBase Invoke(MessageEventArgs telegramMessageEvent)
        {
            ReturnModelBase returnModel = new ReturnModelBase { };

            var thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            string processMemoryUsage = Convert.ToDecimal(thisProcess.WorkingSet64 / 1000000).ToString();
            DateTime processStartTime = thisProcess.StartTime;
            string systemHostname = System.Net.Dns.GetHostName();
            string systemOpSys = "(Unknown OS)";
            string systemOpSysVersion = String.Empty;
            string systemTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzz");
            string userId = TelegramUtilities.GetUserId(telegramMessageEvent).ToString();
            string userLanguageCode = TelegramUtilities.GetUserLanguageCode(telegramMessageEvent);

            TimeSpan timeSinceStart = DateTime.Now.ToUniversalTime().Subtract(processStartTime.ToUniversalTime());
            string uptime = timeSinceStart.ToString("d'd 'h'h 'm'm 's's'");

            int queueAmountInt = QueueData.CountQueuedQueueItems();
            string queueAmount = queueAmountInt.ToString();
            int queueSleepTime = QueueUtilities.GetSleepTime(queueAmountInt);
            string queueApproxDays = TimeSpan.FromMilliseconds(queueAmountInt*queueSleepTime).TotalDays.ToString("0");

            string runtime = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName
                .Replace(".NETCoreApp", ".NET Core")
                .Replace(",Version=v", " ");

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                systemOpSys = "Windows";

                if (System.Environment.OSVersion.Version.Build > 9600)
                {
                    systemOpSysVersion = "10.0." + System.Environment.OSVersion.Version.Build;
                }
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                systemOpSys = "macOS";
                systemOpSysVersion = System.Environment.OSVersion.Version.ToString();
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                systemOpSys = "Linux";
                systemOpSysVersion = System.Environment.OSVersion.Version.ToString();
            }

            string outputText = $@"<b>StashBot</b> | {AppVersion.FullVersion}
—
<b>⚙️ Bot</b>
Memory: <code>{processMemoryUsage}mb</code>
️Uptime: <code>{uptime}</code>
Env.: <code>{runtime}</code>
—
<b>📊 Stats</b>
Queue: <code>{queueAmount}</code> <i>(~{queueApproxDays} days)</i>
—
<b>🖥️ System</b>
Host: <code>{systemHostname}</code>
OS: <code>{systemOpSys} {systemOpSysVersion}</code>
Time: <code>{systemTime}</code>
—
<b>👤 You</b>
ID: <code>{userId}</code>
Language: <code>{userLanguageCode}</code>";

            returnModel.SendTextMessageArguments = new SendTextMessageArguments
            {
                Text = outputText
            };

            return returnModel;
        }
    }
}