using System;
using System.Collections.Generic;
using StashBot.Models;
using StashBot.Utilities;

namespace StashBot.Services.ScrapeServices
{
    public class TwitterScrapeService
    {
        public QueueItem ScrapeTwitterUrl(string url, int mediaIndex, string name)
        {
            QueueItem returnItem = null;

            url = url.Replace("https://mobile.twitter", "https://twitter");

            if (url.StartsWith("https://twitter.com"))
            {
                var galleryDlOutput = GalleryDlService.GetJsonFromUrl(url);

                bool hasMedia = false;
                string nameFromApi = String.Empty;
                string source = String.Empty;
                string username = String.Empty;
                List<string> media = new List<string>();
                QueueItem.MediaType mediaType = QueueItem.MediaType.Image;

                foreach (var item in galleryDlOutput.Children())
                {
                    if(item[0].ToString() == "ValueError")
                    {
                        MessageUtilities.PrintHackMessage("Running ScrapeTwitterUrl() again to combat gallery-dl issue");
                        // HACK: gallery-dl sometimes spits out
                        // [
                        //   [
                        //     "ValueError",
                        //     "substring not found"
                        //   ]
                        // ]
                        // so we'll just keep trying until it works
                        // TODO: Not this. This is awful. No no no.
                        TwitterScrapeService _twitterScrapeService = new TwitterScrapeService();
                        return _twitterScrapeService.ScrapeTwitterUrl(url, mediaIndex, name);
                    }

                    if(item[0].ToString() == "HttpError")
                    {
                        throw new Exception("Failure to connect to Twitter");
                    }

                    int type = Convert.ToInt32(item[0]);

                    switch (type)
                    {
                        case 2:
                            nameFromApi = item[0].Next["author"]["nick"].ToString();
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
                    name = String.IsNullOrEmpty(name) ? nameFromApi : name;
                    source = $"https://twitter.com/{username}/status/{source}";
                    username = $"https://twitter.com/{username}";

                    var selectedMedia = (mediaIndex + 1 > media.Count) ? media[0] : media[mediaIndex];

                    returnItem = new QueueItem
                    {
                        MediaUrl = selectedMedia,
                        Name = name,
                        SourceName = "Twitter",
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
                throw new Exception("URL is not compatible with Twitter scraper");
            }

            return returnItem;
        }
    }
}