using GearstashBot.Models;
using GearstashBot.Models.ArgumentModels;
using GearstashBot.Services;
using GearstashBot.Utilities;

namespace GearstashBot.Handlers.CommandHandlers
{
    public class CatPlsCommandHandler
    {
        public static void Invoke(CommandHandlerArguments arguments)
        {
            string height = GeneratorUtilities.GenerateRandomNumber(250, 1000).ToString();
            string width = GeneratorUtilities.GenerateRandomNumber(250, 1000).ToString();

            TelegramApiService.SendPhoto(
                new SendPhotoArguments
                {
                    Photo = $"https://placekitten.com/{height}/{width}"
                },
                Program.BotClient,
                arguments.TelegramMessageEvent
            );

            Constants.Cats++;
        }
    }
}