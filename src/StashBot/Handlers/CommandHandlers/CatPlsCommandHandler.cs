using StashBot.Models.ArgumentModels;
using StashBot.Models.ReturnModels.CatPlsCommandHandlerReturns;
using StashBot.Utilities;

namespace StashBot.Handlers.CommandHandlers
{
    public class CatPlsCommandHandler
    {
        public static CatPlsInvokeReturn Invoke()
        {
            CatPlsInvokeReturn returnModel = new CatPlsInvokeReturn {};

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