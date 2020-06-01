
namespace StashBot.Models.ArgumentModels
{
    public class SendPhotoArguments : TelegramApiServiceBase
    {
        public string Caption { get; set; }
        public string Photo { get; set; }
    }
}