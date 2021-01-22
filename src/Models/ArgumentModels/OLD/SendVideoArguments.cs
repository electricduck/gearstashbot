
namespace GearstashBot.Models.ArgumentModels
{
    public class SendVideoArguments : TelegramApiServiceBase
    {
        public string Caption { get; set; }
        public string Video { get; set; }
    }
}