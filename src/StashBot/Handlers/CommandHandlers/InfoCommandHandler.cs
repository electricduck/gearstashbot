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

            var version = ReflectionUtilities.GetVersion();

            string processMemoryUsage = Convert.ToDecimal(thisProcess.WorkingSet64 / 1000000).ToString();
            DateTime processStartTime = thisProcess.StartTime;
            string systemHostname = System.Net.Dns.GetHostName();
            string systemOpSys = "(Unknown OS)";
            string systemOpSysVersion = String.Empty;
            string systemTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzz");
            string userId = arguments.TelegramUser.Id.ToString();
            string userLanguageCode = arguments.TelegramUser.Language.ToString();

            TimeSpan timeSinceStart = DateTime.Now.ToUniversalTime().Subtract(processStartTime.ToUniversalTime());
            string uptime = timeSinceStart.ToString("d'd 'h'h 'm'm 's's'");

            int queueAmountInt = QueueData.CountQueuedQueueItems();
            string queueAmount = queueAmountInt.ToString();
            string queueApproxDays = QueueUtlities.CalculateQueueApproxDays(queueAmountInt).ToString("0.00");
            string totalQueueAmount = QueueData.CountQueueItems().ToString();

            string runtime = ParseRuntime(Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName);

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

            string outputText = $@"<b>StashBot</b> | {version}
—
<b>⚙️ Bot</b>
Memory: <code>{processMemoryUsage}mb</code>
️Uptime: <code>{uptime}</code>
Env.: <code>{runtime}</code>
—
<b>📊 Stats</b>
Queue: <code>{queueAmount}</code> <i>(~{queueApproxDays} days)</i>
Total: <code>{totalQueueAmount}</code>
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
    
        private static string ParseRuntime(string framework)
        {
            string name = "";
            double version = Convert.ToDouble(
                framework
                    .Replace(".NETCoreApp,Version=v", "")
            );

            if(version >= 5.0)
            {
                name = ".NET";
            } else {
                name = ".NET Core";
            }

            return $"{name} {version.ToString("0.0")}";
        }
    }
}