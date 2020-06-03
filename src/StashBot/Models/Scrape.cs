using System.Collections.Generic;

namespace StashBot.Models
{
    public class Scrape : QueueItem
    {
        public List<string> Media { get; set; } = new List<string>();
        public bool HasMedia { get; set; }
        public string SourceId { get; set; }
        public string Username { get; set; }
    }
}