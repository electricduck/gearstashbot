using System;
using Newtonsoft.Json.Linq;
using GearstashBot.Models;

namespace GearstashBot.Services
{
    public class ScrapeService
    {
        public Scrape Scrape(string url)
        {
            Scrape scrape = new Scrape { };

            url = url
                .Replace("https://mobile.twitter.com", "https://twitter.com")
                .Replace("https://nitter.net", "https://twitter.com");

            JArray galleryDlOutput = null;

            try
            {
                galleryDlOutput = GalleryDlService.GetJsonFromUrl(url);
            }
            catch
            {
                return null;
            }

            foreach (var item in galleryDlOutput.Children())
            {
                string possibleException = item[0].ToString();
                switch (possibleException)
                {
                    case "HttpError":
                        throw new Exception($"Failed to connect to {url}");
                    case "ValueError":
                        throw new Exception($"Failed to download item");
                }

                Source source = new Source();

                switch (Convert.ToInt32(item[0]))
                {
                    case 2:
                        if (item[0].Next["category"] != null)
                        {
                            source = GetSource(item[0].Next["category"].ToString());
                            scrape.SourceName = source.Name;
                        }

                        if (item[0].Next["owner"] != null)
                        {
                            if (item[0].Next["owner"]["nsid"] != null)
                                scrape.Username = item[0].Next["owner"]["nsid"].ToString(); // Flickr

                            if (item[0].Next["owner"]["realname"] != null)
                                scrape.Name = item[0].Next["owner"]["realname"].ToString(); // Flickr
                        }
                        else if (item[0].Next["user"] != null)
                        {
                            if (item[0].Next["user"]["nick"] != null)
                                scrape.Name = item[0].Next["user"]["nick"].ToString(); // Twitter

                            if (item[0].Next["user"]["name"] != null)
                                scrape.Username = item[0].Next["user"]["name"].ToString(); // Twitter
                        }
                        else if (item[0].Next["username"] != null)
                        {
                            scrape.Username = item[0].Next["username"].ToString(); // Instagram;
                        }

                        if (item[0].Next["post_shortcode"] != null) // TODO: Fix SourceId not being set properly
                        {
                            scrape.SourceId = item[0].Next["post_shortcode"].ToString();
                        }
                        else if (item[0].Next["tweet_id"] != null)
                        {
                            scrape.SourceId = item[0].Next["tweet_id"].ToString(0);
                        }

                        if (scrape.Name == null)
                            scrape.Name = scrape.Username;

                        break;
                    case 3:
                        Media extractedMedia = new Media();
                        extractedMedia.Type = QueueItem.MediaType.Image;

                        if (item[2]["extension"] != null) // Twitter                        
                        {
                            switch (item[2]["extension"].ToString())
                            {
                                case "jpeg":
                                case "jpg":
                                    extractedMedia.Type = QueueItem.MediaType.Image;
                                    break;
                                case "mp4":
                                    extractedMedia.Type = QueueItem.MediaType.Video;
                                    break;
                            }
                        }
                        else if (item[2]["typename"] != null) // Instagram
                        {
                            switch (item[2]["typename"].ToString())
                            {
                                case "GraphImage":
                                    extractedMedia.Type = QueueItem.MediaType.Image;
                                    break;
                                case "GraphVideo":
                                    extractedMedia.Type = QueueItem.MediaType.Video;
                                    break;
                            }
                        }

                        if (
                            item[2]["urls"] != null &&
                            item[2]["urls"]["url"] != null &&
                            item[2]["urls"]["url"][0]["_content"] != null
                        ) // Flickr
                        {
                            extractedMedia.SourceUrl = item[2]["urls"]["url"][0]["_content"].ToString();
                        }
                        else if(item[2]["post_url"] != null) // Instagram
                        {
                            extractedMedia.SourceUrl = item[2]["post_url"].ToString();
                        }
                        else if(item[2]["tweet_id"] != null) // Twitter
                        {
                            extractedMedia.SourceUrl = GetSource("twitter").SourceUrlTemplate // HACK: Until problem above is resolved
                                .Replace("{id}", item[2]["tweet_id"].ToString())
                                .Replace("{username}", item[2]["user"]["name"].ToString());
                        }

                        if (item[1] != null)
                        {
                            extractedMedia.MediaUrl = item[1].ToString();
                            scrape.Media.Add(extractedMedia);
                            scrape.HasMedia = true;
                        }

                        break;
                }

                if (String.IsNullOrEmpty(scrape.UsernameUrl))
                    scrape.UsernameUrl = ParseSourceUrl(source.UsernameUrlTemplate, scrape);
            }

            return scrape;
        }

        private Source GetSource(string sourceName)
        {
            Source returnModel = new Source();

            switch (sourceName.ToLower())
            {
                case "flickr":
                    returnModel.Name = "Flickr";
                    returnModel.SourceUrlTemplate = "https://www.flickr.com/photos/{username}/{id}/";
                    returnModel.UsernameUrlTemplate = "https://www.flickr.com/photos/{username}/";
                    break;
                case "instagram":
                    returnModel.Name = "Instagram";
                    returnModel.SourceUrlTemplate = "https://www.instagram.com/p/{id}/";
                    returnModel.UsernameUrlTemplate = "https://www.instagram.com/{username}/";
                    break;
                case "twitter":
                    returnModel.Name = "Twitter";
                    returnModel.SourceUrlTemplate = "https://twitter.com/{username}/status/{id}";
                    returnModel.UsernameUrlTemplate = "https://twitter.com/{username}";
                    break;
                default:
                    returnModel.Name = sourceName;
                    break;
            }

            return returnModel;
        }

        private string ParseSourceUrl(string template, Scrape scrape)
        {
            return template
                .Replace("{id}", scrape.SourceId)
                .Replace("{username}", scrape.Username);
        }
    }
}