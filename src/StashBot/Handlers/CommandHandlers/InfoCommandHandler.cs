using System;
using System.Reflection;
using System.Runtime.Versioning;
using StashBot.Data;
using StashBot.Models;
using StashBot.Models.ArgumentModels;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class InfoCommandHandler
    {
        public static Help Help = new Help {
            Command = "info",
            Description = "Bot information and status"
        };

        public static void Invoke(CommandHandlerArguments arguments)
        {
            var thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            string processMemoryUsage = Convert.ToDecimal(thisProcess.WorkingSet64 / 1000000).ToString();
            DateTime processStartTime = thisProcess.StartTime;
            string systemHostname = System.Net.Dns.GetHostName();
            string systemOpSys = "(Unknown OS)";
            string systemOpSysVersion = String.Empty;
            string systemTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzz");
            string userId = TelegramUtilities.GetUserId(arguments.TelegramMessageEvent).ToString();
            string userLanguageCode = TelegramUtilities.GetUserLanguageCode(arguments.TelegramMessageEvent);

            TimeSpan timeSinceStart = DateTime.Now.ToUniversalTime().Subtract(processStartTime.ToUniversalTime());
            string uptime = timeSinceStart.ToString("d'd 'h'h 'm'm 's's'");

            int queueAmountInt = QueueData.CountQueuedQueueItems();
            string queueAmount = queueAmountInt.ToString();
            string queueApproxDays = TimeSpan.FromMilliseconds(queueAmountInt*AppSettings.Config_PostInterval).TotalDays.ToString("0.00"); // BUG: This breaks after a large amount of days and starts showing a negative value

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