using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using StashBot.Models;

namespace StashBot.Services.ScrapeServices
{
    public class TwitterScrapeService
    {
        private string service = "Twitter";
        private string nitterUrl = "https://nitter.zyrio.app";

        // TODO: Handle the possibility Twitter allows mixed video/image albums
        public QueueItem ScrapeTwitterUrl(string url, int mediaIndex, string customName)
        {
            QueueItem returnItem = null;

            url = url.Replace("https://mobile.twitter.com", "https://twitter.com");

            if (url.StartsWith("https://twitter.com"))
            {
                var galleryDlOutput = GalleryDlService.GetJsonFromUrl(url);

                bool hasMedia = false;

                string name = String.Empty;
                string source = String.Empty;
                string username = String.Empty;

                List<string> media = new List<string>();
                QueueItem.MediaType mediaType = QueueItem.MediaType.Image;

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
                            name = item[0].Next["author"]["nick"].ToString();
                            source = item[0].Next["tweet_id"].ToString(0);
                            username = item[0].Next["author"]["name"].ToString();
                            break;
                        case 3:
                            media.Add(YoutubeDlService.GetExtractedPathFromUrl(url));
                            mediaType = QueueItem.MediaType.Video;
                            hasMedia = true;
                            break;
                        case 7:
                            media.Add(item[1][0].ToString().Replace(":orig", ""));
                            mediaType = QueueItem.MediaType.Image;
                            hasMedia = true;
                            break;
                    }
                }

                if (hasMedia)
                {
                    name = String.IsNullOrEmpty(customName) ? name : customName;
                    source = $"https://twitter.com/{username}/status/{source}";
                    username = $"https://twitter.com/{username}";

                    var selectedMedia = (mediaIndex + 1 > media.Count) ? media[0] : media[mediaIndex];

                    returnItem = new QueueItem
                    {
                        MediaUrl = selectedMedia,
                        Name = name,
                        SourceName = service,
                        SourceUrl = source,
                        Type = mediaType,
                        UsernameUrl = username
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

        public QueueItem ScrapeNitterUrl(string url, int mediaIndex, string customName)
        {
            QueueItem returnItem = null;

            url = url.Replace("https://mobile.twitter.com", "https://twitter.com");

            if (url.StartsWith("https://twitter.com"))
            {
                url = url.Replace("https://twitter.com", nitterUrl);
            }

            if (url.StartsWith(nitterUrl) || url.StartsWith("https://nitter.net"))
            {
                bool hasMedia = false;

                List<string> media = new List<string>();
                QueueItem.MediaType mediaType = QueueItem.MediaType.Image;

                var web = new HtmlWeb();
                var document = web.Load(url);
                var documentNode = document.DocumentNode;

                string name = documentNode.SelectNodes("//div[contains(@class, 'main-tweet')]//a[contains(@class, 'fullname')]")[0].InnerText;
                string username = "https://twitter.com/" + documentNode.SelectNodes("//div[contains(@class, 'main-tweet')]//a[contains(@class, 'username')]")[0].InnerText.Replace("@", "");
                string source = documentNode.SelectNodes("//div[contains(@class, 'nav-item')]//a[contains(@class, 'icon-bird')]")[0].Attributes["href"].Value;

                HtmlNodeCollection extractedImages = documentNode.SelectNodes("//div[contains(@class, 'main-tweet')]//a[contains(@class, 'still-image')]");
                HtmlNodeCollection extractedVideos = documentNode.SelectNodes("//div[contains(@class, 'main-tweet')]//div[contains(@class, 'gallery-video')]");

                if(extractedImages != null)
                {
                    foreach (var extractedImage in extractedImages)
                    {
                        media.Add(nitterUrl + extractedImage.Attributes["href"].Value);
                        mediaType = QueueItem.MediaType.Image;
                        hasMedia = true;
                    }
                }

                if(extractedVideos != null)
                {
                    media.Add(YoutubeDlService.GetExtractedPathFromUrl(source));
                    mediaType = QueueItem.MediaType.Video;
                    hasMedia = true;
                }

                if (hasMedia)
                {
                    var selectedMedia = (mediaIndex + 1 > media.Count) ? media[0] : media[mediaIndex];

                    returnItem = new QueueItem
                    {
                        MediaUrl = selectedMedia,
                        Name = name,
                        SourceName = service,
                        SourceUrl = source,
                        Type = mediaType,
                        UsernameUrl = username
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