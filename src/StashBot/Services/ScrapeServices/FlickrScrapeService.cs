using System;
using System.Collections.Generic;
using StashBot.Models;

namespace StashBot.Services.ScrapeServices
{
    public class FlickrScrapeService
    {
        private string service = "Flickr";

        public QueueItem ScrapeFlickrUrl(string url, int mediaIndex)
        {
            QueueItem returnItem = null;

            if (url.StartsWith("https://flickr.com") ||
                url.StartsWith("https://www.flickr.com/"))
            {
                var galleryDlOutput = GalleryDlService.GetJsonFromUrl(url);

                bool hasMedia = false;

                string name = String.Empty;
                string source = String.Empty;
                string username = String.Empty;

                List<string> media = new List<string>();
                List<string> sources = new List<string>();
                QueueItem.MediaType mediaType = QueueItem.MediaType.Image;

                foreach (var item in galleryDlOutput.Children())
                {
                    switch (Convert.ToInt32(item[0]))
                    {
                        case 2:
                            name = item[0].Next["owner"]["realname"].ToString();
                            username = item[0].Next["owner"]["nsid"].ToString();

                            if (String.IsNullOrEmpty(name))
                            {
                                name = item[0].Next["owner"]["username"].ToString();
                            }

                            if(!String.IsNullOrEmpty(item[0].Next["owner"]["path_alias"].ToString()))
                            {
                                username = item[0].Next["owner"]["path_alias"].ToString();
                            }
                            break;
                        case 3:
                            media.Add(item[1].ToString());
                            sources.Add(item[2]["urls"]["url"][0]["_content"].ToString());
                            hasMedia = true;
                            break;
                    }
                }

                if(hasMedia)
                {
                    username = $"https://www.flickr.com/photos/{username}/";

                    var selectedMedia = media[0];
                    var selectedSource = sources[0];

                    returnItem = new QueueItem
                    {
                        MediaUrl = (mediaIndex + 1 > media.Count || mediaIndex < 0) ?
                            media[0] :
                            media[mediaIndex],
                        Name = name,
                        SourceName = service,
                        SourceUrl = (mediaIndex + 1 > sources.Count || mediaIndex < 0) ?
                            sources[0] :
                            sources[mediaIndex],
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