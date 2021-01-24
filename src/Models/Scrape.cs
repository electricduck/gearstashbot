using System.Collections.Generic;

namespace GearstashBot.Models
{
    public class Scrape : QueueItem
    {
        public List<Media> Media { get; set; } = new List<Media>();
        public bool HasMedia { get; set; }
        public string UrlCategory { get; set; }
        public string UrlId { get; set; }
        public string UrlSlug { get; set; }
        public string Username { get; set; }
    }
}