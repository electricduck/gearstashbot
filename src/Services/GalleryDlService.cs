using System.Diagnostics;
using Newtonsoft.Json.Linq;
using GearstashBot.Models;

namespace GearstashBot.Services
{
    public class GalleryDlService
    {
        public static JArray GetJsonFromUrl(
            string url
        )
        {
            string galleryDlOutput = Invoke($"{url} -j");
            JArray parsedJson = JArray.Parse(galleryDlOutput);
            return parsedJson;
        }

        private static string Invoke(string args)
        {
            // TODO: Handle gallery-dl not installed
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "gallery-dl",
                    Arguments = $"--clear-cache {args}",
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