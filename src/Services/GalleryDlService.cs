using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace GearstashBot.Services
{
    public class GalleryDlService
    {
        public static JArray GetJsonFromUrl(string url)
        {
            string galleryDlOutput = Invoke($"{url} -j");
            JArray parsedJson = JArray.Parse(galleryDlOutput);
            return parsedJson;
        }

        private static string Invoke(string args)
        {
            if (File.Exists($"config/cookies.txt"))
                args += " --cookies ./config/cookies.txt";

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