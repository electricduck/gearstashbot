using StashBot.Models.ArgumentModels;
using StashBot.Services;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
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
        }
    }
}