using System.Collections.Generic;
using System.Linq;
using GearstashBot.Data;
using GearstashBot.I18n;
using GearstashBot.Models;
using GearstashBot.Models.ArgumentModels;
using GearstashBot.Utilities;

namespace GearstashBot.Handlers.CommandHandlers
{
    public class AnnounceCommandHandler
    {
        // TODO: Detect errors
        public static void Invoke(CommandHandlerArguments arguments, bool reload = false)
        {
            if (AuthorData.CanAuthorAnnounce(arguments.TelegramUser.Id))
            {
                List<Author> authors = AuthorData
                    .GetAuthors()
                    .Where(a => a.CanQueue == true)
                    .Where(a => a.QueueItems.Count > 0) // TODO: "Active" status?
                    .ToList();

                foreach (Author author in authors)
                {
                    MessageUtilities.SendWarningMessage(
                        Localization.GetPhrase(
                            Localization.Phrase.Announcement,
                            arguments.TelegramUser,
                            new string[] {
                                arguments.CommandArgument
                            }
                        ),
                        author.TelegramId
                    );
                }
            }
        }
    }
}