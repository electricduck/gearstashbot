using System.Collections.Generic;

namespace GearstashBot.Models
{
    public class Scrape : QueueItem
    {
        public List<Media> Media { get; set; } = new List<Media>();
        public bool HasMedia { get; set; }
        public string SourceId { get; set; }
        public string Username { get; set; }
    }
}