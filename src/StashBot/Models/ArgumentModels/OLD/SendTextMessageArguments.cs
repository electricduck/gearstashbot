
namespace StashBot.Models.ArgumentModels
{
    public class SendTextMessageArguments : TelegramApiServiceBase
    {
        public bool DisableWebPagePreview { get; set; } = true;
        public string Text { get; set; }
    }
}