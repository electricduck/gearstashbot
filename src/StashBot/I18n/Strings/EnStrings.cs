using System.Collections.Generic;

namespace StashBot.I18n.Strings
{
    public class EnStrings
    {
        public static Dictionary<Localization.Phrase, string> StringDictionary = new Dictionary<Localization.Phrase, string>() {
            { Localization.Phrase.AlreadyBeenQueued, "This has already been queued" },
            { Localization.Phrase.CannotFindAuthor, "Cannot find <code>[0]</code>"},
            { Localization.Phrase.CreatedNewAuthor, "New user has started the bot, <b>[0]</b> (<code>[1]</code>). Set permissions with <code>/user [1]</code>." },
            { Localization.Phrase.Delete, "Delete" },
            { Localization.Phrase.DeletedFromQueue, "Deleted from queue"},
            { Localization.Phrase.Later, "Later" },
            { Localization.Phrase.Latest, "Latest" },
            { Localization.Phrase.LinkContainsNoMedia, "This link contains no media or does not exist" },
            { Localization.Phrase.NothingIsQueued, "Nothing is in selected queue" },
            { Localization.Phrase.NoPermissionPostQueue, "You do not have permission to queue new posts" },
            { Localization.Phrase.NoPermissionRemovePost, "You do not have permission to remove this post" },
            { Localization.Phrase.NoPermissionViewQueue, "You do not have permission to view the queue"},
            { Localization.Phrase.PostSuccessfullyQueued, "Post successfully queued" },
            { Localization.Phrase.ServiceNotSupported, "This service is not supported"},
            { Localization.Phrase.Sooner, "Sooner" },
            { Localization.Phrase.Soonest, "Soonest" },
            { Localization.Phrase.WelcomeFirstAuthor, $"<b>Welcome to StashBot, [0]!</b><br />Set your permissions with <code>/user [0]</code> (or just <code>/user</code>)." }
        };
    }
}