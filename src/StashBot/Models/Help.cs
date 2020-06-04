using System.Collections.Generic;

namespace StashBot.Models
{
    public class Help
    {
        public List<HelpArgument> Arguments { get; set; }
        public string Command { get; set; }
        public string Description { get; set; }
        public bool DescriptionIsItalicized { get; set; } = true;
    }
}