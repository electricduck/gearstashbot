using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace StashBot.Services
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

        static string Invoke(string args)
        {
            // TODO: Handle gallery-dl not installed
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "gallery-dl",
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