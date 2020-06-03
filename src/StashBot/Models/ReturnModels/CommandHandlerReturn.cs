
using StashBot.Models.ArgumentModels;

namespace StashBot.Models.ReturnModels
{
    public class CommandHandlerReturn
    {
        public SendPhotoArguments SendPhotoArguments { get; set; }
        public SendTextMessageArguments SendTextMessageArguments { get; set; }
        public SendVideoArguments SendVideoArguments { get; set; }
    }
}