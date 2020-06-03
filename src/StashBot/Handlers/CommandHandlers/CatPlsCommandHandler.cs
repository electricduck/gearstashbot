using StashBot.Models.ArgumentModels;
using StashBot.Models.ReturnModels;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class CatPlsCommandHandler
    {
        public static CommandHandlerReturn Invoke()
        {
            CommandHandlerReturn returnModel = new CommandHandlerReturn {};

            string height = GeneratorUtilities.GenerateRandomNumber(250, 1000).ToString();
            string width = GeneratorUtilities.GenerateRandomNumber(250, 1000).ToString();

            returnModel.SendPhotoArguments = new SendPhotoArguments
            {
                Photo = $"https://placekitten.com/{height}/{width}"
            };

            return returnModel;
        }
    }
}