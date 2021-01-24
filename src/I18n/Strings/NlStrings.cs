using System.Collections.Generic;

namespace GearstashBot.I18n.Strings
{
    public class NlStrings
    {
        public static Dictionary<Localization.Phrase, string> StringDictionary = new Dictionary<Localization.Phrase, string>() {
            { Localization.Phrase.AlreadyBeenQueued, "Het staat al in de wachtrij" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Author, "Poster" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Back, "Terug" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.CannotDeleteXFromChannel, "Kan #[0] niet verwijderen uit kanaal"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.CannotFetchFile, "Telegram kan dit bestand niet ophalen (<code> [1] </code>): <br /> [0]"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.CannotFindAuthor, "<code>[0]</code> kan niet worden gevonden"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.CannotFindMessageX, "Kan bericht #<code>[0]</code> niet vinden"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.CannotDeleteTemporarilyDueToLongRunningRequest, "Kan dit item niet verwijderen vanwege een langlopend verzoek. Probeer het over een paar seconden opnieuw."}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.CannotPostTemporarilyDueToLongRunningRequest, "Kan niet posten in de wachtrij vanwege een langlopend verzoek. Probeer het over een paar seconden opnieuw."}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.CannotRemovePermissionFromSelf, "U kunt deze toestemming niet bij uzelf intrekken"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.CreatedNewAuthor, "Nieuwe gebruiker heeft de bot gestart, <b>[0]</b> (<code>[1]</code>). Stel rechten in met <code>/user [1]</code>." }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Delete, "Verwijderen" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Deleted, "Verwijderd" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.DeleteOthers, "Verwijder anderen" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.DeletedXFromChannel, "#[0] verwijderd van kanaal"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Earlier, "Eerder" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Earliest, "Vroegste" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.FlushDanglingUsers, "Zuiver hangende gebruikers" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.FlushedXDanglingUsers, "[0] bungelende gebruikers verwijderd" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.InvalidArgs, "Ongeldige argumenten" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Language, "Taal" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.LastAccessed, "Laatst geopend" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Later, "Later" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Latest, "Laatste" }, // ✔️ GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.LinkContainsNoMedia, "Deze link bevat geen media of bestaat niet" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.LoadingQueue, "Wachtrij laden ..." }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.ManageUsers, "Manager gebruikers" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.MessageID, "Bericht-ID" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Name, "Naam" }, // ✔️ GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NoDanglingUsers, "Geen bungelende gebruikers om te zuiveren" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NotSet, "Niet ingesteld" }, // ✔️ GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NothingIsDeleted, "Er is niets verwijderd" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NothingIsPosted, "Er is niets gepost" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NothingIsQueued, "Niets staat in de wachtrij" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NoPermissionFlushDanglingUsers, "U hebt geen toestemming om hangende gebruikers te verwijderen" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NoPermissionManageUsers, "U heeft geen toestemming om gebruikers te beheren"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NoPermissionPostQueue, "Je hebt geen toestemming om nieuwe berichten in de wachtrij te plaatsen" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NoPermissionRandomizeQueue, "U heeft geen toestemming om de wachtrij in willekeurige volgorde af te spelen"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NoPermissionRemovePost, "Je hebt geen toestemming om dit bericht te verwijderen" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NoPermissionTools, "U heeft geen toestemming om tools te gebruiken" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.NoPermissionViewQueue, "U heeft geen toestemming om de wachtrij te bekijken"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.PostSuccessfullyQueued, "Post succesvol in wachtrij geplaatst" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Posted, "Geplaatst" }, // ✔️ GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Posts, "Berichten" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.ProfileUpdated, "Profiel geüpdatet"}, // ✔️ GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Queue, "Wachtrij" }, // ✔️ GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Queued, "In de wachtrij"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.RandomizeQueue, "Shuffle"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.RandomizedQueue, "Geschudde wachtrij"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Retry, "Herzein" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.ServiceNotSupported, "Deze service wordt niet ondersteund"}, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Sooner, "Eerder" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Soonest, "Snelst" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Tools, "Hulpmiddelen" }, // ✔️ GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.User, "Gebruiker" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Username, "Gebruikersnaam" }, // GTranslate [En->Nl] 24-Jan-2021
            { Localization.Phrase.Viewing, "Bekijken" }, // GTranslate [En->Nl] 24-Jan-2021
        };
    }
}