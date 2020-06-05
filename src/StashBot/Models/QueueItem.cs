using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StashBot.Models
{
    public class QueueItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string MediaUrl { get; set; }
        public string Name { get; set; }
        public string SourceName { get; set; }
        public string SourceUrl { get; set; }
        public string UsernameUrl { get; set; }

        public QueueStatus Status { get; set; }
        public MediaType Type { get; set; }

        public DateTime DeletedAt { get; set; }
        public DateTime PostedAt { get; set; }
        public DateTime QueuedAt { get; set; }

        public int MessageId { get; set; }

        public Author Author { get; set; }

        public enum QueueStatus
        {
            Any = 100,
            Created = 0,
            Queued = 2,
            Disputed = 4,
            Deleted = 5,
            Posted = 3
        }

        public enum MediaType
        {
            Image,
            Video
        }
    }
}