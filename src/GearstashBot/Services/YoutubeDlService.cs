using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace GearstashBot.Services
{
    public class YoutubeDlService
    {
        public static string GetExtractedPathFromUrl(
            string url
        )
        {
            string youtubeDlOutput = Invoke($"-g {url}");
            return youtubeDlOutput.Trim();
        }

        private static string Invoke(string args)
        {
            // TODO: Handle gallery-dl not installed
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "youtube-dl",
                    Arguments = $"{args}",
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