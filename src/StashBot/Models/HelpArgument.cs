
namespace StashBot.Models
{
    public class HelpArgument
    {
        public string Example { get; set;}
        public string Explanation { get; set; }
        public string Name { get; set; }
        public bool Optional { get; set; }
        public int Position { get; set; }   
    }
}