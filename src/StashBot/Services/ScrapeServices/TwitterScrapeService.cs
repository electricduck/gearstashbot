using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using StashBot.Models;
using StashBot.Utilities;

namespace StashBot.Services.ScrapeServices
{
    public class TwitterScrapeService
    {
        private string service = "Twitter";

        // TODO: Handle the possibility Twitter allows mixed video/image albums
        public QueueItem ScrapeTwitterUrl(string url, int mediaIndex, string customName)
        {
            QueueItem returnItem = null;

            url = url
                .Replace("https://mobile.twitter.com", "https://twitter.com")
                .Replace("https://nitter.net", "https://twitter.com");

            if (url.StartsWith("https://twitter.com"))
            {
                var galleryDlOutput = GalleryDlService.GetJsonFromUrl(url);

                Scrape extracted = new Scrape { };

                foreach (var item in galleryDlOutput.Children())
                {
                    if (item[0].ToString() == "ValueError")
                    {
                        throw new Exception($"Failed to download item from {service}");
                    }

                    if (item[0].ToString() == "HttpError")
                    {
                        throw new Exception($"Failed to connect to {service}");
                    }

                    switch (Convert.ToInt32(item[0]))
                    {
                        case 2:
                            extracted.Name = item[0].Next["user"]["nick"].ToString();
                            extracted.SourceId = item[0].Next["tweet_id"].ToString(0);
                            extracted.Username = item[0].Next["user"]["name"].ToString();
                            break;
                        case 3:
                            extracted.Media.Add(item[1].ToString());
                            extracted.Type = QueueItem.MediaType.Video;
                            extracted.HasMedia = true;
                            break;
                        case 7:
                            extracted.Media.Add(item[1][0].ToString().Replace(":orig", ""));
                            extracted.Type = QueueItem.MediaType.Image;
                            extracted.HasMedia = true;
                            break;
                    }
                }

                if (extracted.HasMedia)
                {
                    extracted.Name = String.IsNullOrEmpty(customName) ? extracted.Name : customName;
                    extracted.SourceUrl = (extracted.SourceUrl != null) ? extracted.SourceUrl : $"https://twitter.com/{extracted.Username}/status/{extracted.SourceId}";
                    extracted.UsernameUrl = (extracted.UsernameUrl != null) ? extracted.UsernameUrl : $"https://twitter.com/{extracted.Username}";

                    if (url.Contains("/photo/")) // NOTE: This will cause issues for the Twitter account @photo
                    {
                        mediaIndex = Convert.ToInt32(url.Substring(url.IndexOf("/photo/") + "/photo/".Length)) - 1;
                    }

                    returnItem = new QueueItem
                    {
                        MediaUrl = (mediaIndex + 1 > extracted.Media.Count || mediaIndex < 0) ?
                            extracted.Media[0] :
                            extracted.Media[mediaIndex],
                        Name = extracted.Name,
                        SourceName = service,
                        SourceUrl = extracted.SourceUrl,
                        Type = extracted.Type,
                        UsernameUrl = extracted.UsernameUrl
                    };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception($"URL is not compatible with {service} scraper");
            }

            return returnItem;
        }
    }
}