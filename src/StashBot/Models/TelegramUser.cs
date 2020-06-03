using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StashBot.Models
{
    public class TelegramUser
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }
}