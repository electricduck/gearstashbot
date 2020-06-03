
using StashBot.Models.ArgumentModels;

namespace StashBot.Models.ReturnModels
{
    public class ReturnModelBase
    {
        public bool HasPermission { get; set; } = true;
        public bool InvalidArgs { get; set; } = false;
        public SendPhotoArguments SendPhotoArguments { get; set; }
        public SendTextMessageArguments SendTextMessageArguments { get; set; }
        public SendVideoArguments SendVideoArguments { get; set; }
        public bool Success { get; set; } = true;
    }
}