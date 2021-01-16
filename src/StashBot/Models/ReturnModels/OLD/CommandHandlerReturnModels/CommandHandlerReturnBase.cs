using GearstashBot.Models.ArgumentModels;

namespace GearstashBot.Models.ReturnModels.CommandHandlerReturnModels
{
    public class CommandHandlerReturnBase
    {
        public SendPhotoArguments SendPhotoArguments { get; set; }
        public SendTextMessageArguments SendTextMessageArguments { get; set; }

        public string StatusMessage { get; set; }
    }
}