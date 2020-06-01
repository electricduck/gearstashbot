using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StashBot.Models
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long TelegramId { get; set; }

        public bool CanDeleteOthers { get; set; } = false;
        public bool CanFlushQueue { get; set; } = false;
        public bool CanManageAuthors { get; set; } = false;
        public bool CanQueue { get; set; } = false;
    }
}